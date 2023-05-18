using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ultra.Core.DAL.Extensions;
using Ultra.Core.Services.Abstract;
using Ultra.Extensions;
using Ultra.Infrastructure.Http.Extensions;
using Ultra.Infrastructure.Http.Interfaces;
using Ultra.Infrastructure.Models;
using Ultra.Msg.WebApi.DAL;
using Ultra.Msg.WebApi.DAL.Entities;
using Ultra.Msg.WebApi.Extensions;
using Ultra.Msg.WebApi.Hubs;
using Ultra.Msg.WebApi.Hubs.Clients;
using Ultra.Msg.WebApi.Hubs.Interfaces;
using Ultra.Msg.WebApi.Hubs.Repositories;
using Ultra.Msg.WebApi.Models.Channel;
using Ultra.Msg.WebApi.Models.Constants;
using Ultra.Msg.WebApi.Models.Message;
using Ultra.Msg.WebApi.Services.Interfaces;

namespace Ultra.Msg.WebApi.Services
{
    public class ChannelService : IChannelService
    {
        private readonly IUserAccessor _accessor;
        private readonly MsgDbContext _context;
        private readonly IMessageService _messageService;
        private readonly IAuthApiClient _authApiClient;
        private readonly IHubContext<ChatHub, IChatClient> _hubContext;
        private readonly UserHubRepository _userHubRepository;
        private readonly int _userId;
        private readonly IQueryable<Channel> _repository;

        private Expression<Func<Channel, ChannelFullOutputModel>> ChannelToChannelFullOutputModel =>
            x => new ChannelFullOutputModel
            {
                Id = x.Id,
                Type = x.Type,
                Name = x.Name,
                CreateDate = x.CreateDate,
                CreateUserId = x.CreateUserId,
                Silenced = x.Users.First(m => m.UserId == _userId).Silenced,
                LastMessage = x.Messages
                        .OrderByDescending(m => m.SendDate)
                        .AsQueryable()
                        .Select(MessageToMessageFullOutputModel)
                        .FirstOrDefault(),
                UserIds = x.Users.Select(x => x.UserId).ToArray(),
                UnreadMessagesCount = x.Messages
                        .Where(m =>
                            m.SendUserId != _userId &&
                            m.Users.Any(u =>
                                u.UserId == _userId &&
                                !u.Read))
                        .Count()
            };

        private Expression<Func<Message, MessageFullOutputModel>> MessageToMessageFullOutputModel =>
            x => new MessageFullOutputModel
            {
                Guid = x.Guid,
                Text = x.Text,
                ChannelId = x.ChannelId,
                SendUserId = x.SendUserId,
                SendDate = x.SendDate,
                Received = x.Users.Any(u => u.UserId != _userId && u.Received),
                ReceivedDate = x.Users.Where(u => u.UserId != _userId && u.Received).OrderByDescending(u => u.ReceivedDate).First().ReceivedDate,
                Read = x.Users.Any(u => u.UserId == _userId && u.Read),
                ReadDate = x.Users.Where(u => u.UserId != _userId && u.Read).OrderByDescending(u => u.ReadDate).First().ReadDate,
            };

        public ChannelService(
            IUserAccessor accessor,
            MsgDbContext context,
            IMessageService messageService,
            IAuthApiClient authApiClient,
            IHubContext<ChatHub, IChatClient> hubContext,
            UserHubRepository userHubRepository)
        {
            _accessor = accessor;
            _context = context;
            _messageService = messageService;
            _authApiClient = authApiClient;
            _hubContext = hubContext;
            _userHubRepository = userHubRepository;
            _userId = _accessor.UserId;

            _repository = context.Set<Channel>()
                .Where(x => x.Users.Any(m => m.UserId == _userId));
        }

        public async Task<Result<CollectionPage<ChannelFullOutputModel>>> GetChannels(PageModel? pageModel = null)
        {
            var result = await _repository
                .OrderByDescending(x => x.Messages.Any() 
                    ? x.Messages.Max(m => m.SendDate) 
                    : x.CreateDate)
                .Select(ChannelToChannelFullOutputModel)
                .ToCollectionPageAsync(pageModel);

            var userNames = await _authApiClient.GetUserNames(
                        result.Items.Where(x => x.Type == ChannelType.PRIVATE).Select(x => x.UserIds.Where(m => m != _userId).First())
                .Concat(result.Items.Select(x => x.CreateUserId))
                .Concat(result.Items.Where(x => x.LastMessage != null).Select(x => x.LastMessage!.SendUserId))
            );

            foreach (var item in result.Items)
            {
                item.CreateUserName = userNames.GetUserNameOrDefault(item.CreateUserId);

                if (item.Type == ChannelType.PRIVATE)
                {
                    var companionId = item.UserIds.Where(x => x != _userId).First();
                    item.Name = userNames.GetUserNameOrDefault(companionId);
                }

                if (item.LastMessage != null)
                {
                    item.LastMessage.SendUserName = userNames.GetUserNameOrDefault(item.LastMessage!.SendUserId);
                }
            }

            return result;
        }

        public async Task<Result<ChannelFullOutputModel>> GetChannel(int channelId)
        {
            var channel = await _repository
                .WithId(channelId)
                .Select(ChannelToChannelFullOutputModel)
                .FirstOrDefaultAsync();

            if (channel == null)
            {
                return Result.NotFound<ChannelFullOutputModel>();
            }

            await Task.WhenAll(
                Task.Run(async () =>
                {
                    if (channel.Type == ChannelType.PRIVATE)
                    {
                        var companionId = channel.UserIds.Except(_userId.ToEnumerable()).Select(x => (int?)x).FirstOrDefault();
                        if (companionId != null)
                        {
                            channel.Name = (await _authApiClient.GetUserName(companionId.Value))
                                .GetObjectOrDefault(UserConstants.UndefinedUserName);
                        }
                    }
                }),
                Task.Run(async () =>
                {
                    channel.CreateUserName = (await _authApiClient.GetUserName(channel.CreateUserId))
                        .GetObjectOrDefault(UserConstants.UndefinedUserName);
                }),
                Task.Run(async () =>
                {
                    if (channel.LastMessage != null)
                    {
                        channel.LastMessage.SendUserName = (await _authApiClient.GetUserName(channel.LastMessage.SendUserId))
                            .GetObjectOrDefault(UserConstants.UndefinedUserName);
                    }
                }));

            return channel;
        }

        public async Task<Result<CollectionPage<MessageFullOutputModel>>> GetMessages(int channelId, PageModel? model = null)
        {
            var isChannelExists = await _repository.WithId(channelId).AnyAsync();
            if (!isChannelExists)
                return Result.Failed<CollectionPage<MessageFullOutputModel>>();

            var collection = await _context.Set<Message>()
                .Where(x => x.ChannelId == channelId)
                .OrderByDescending(x => x.SendDate)
                .Select(MessageToMessageFullOutputModel)
                .ToCollectionPageAsync(model);

            var userNames = await _authApiClient.GetUserNames(collection.Items.Select(x => x.SendUserId));
            collection.Items.ForEach(x => x.SendUserName = userNames.GetUserNameOrDefault(x.SendUserId));

            return collection;
        }

        public async Task<Result<int>> GetUnreadMessagesCount() 
        {
            var unreadMessagesCount = await _repository
                .SumAsync(x => x.Messages.Where(m => m.SendUserId != _userId && !m.Users.Any(u => u.UserId == _userId && u.Read)).Count());
            return unreadMessagesCount;
        }

        public async Task<Result<ChannelOutputModel>> CreateChannel(ChannelCreateModel model)
        {
            if (model.Type == ChannelType.PRIVATE)
            {
                if (model.UserIds.Count != 1)
                    return Result.Failed<ChannelOutputModel>("Список пользователей должен быть длиной 1");
                var companionId = model.UserIds[0];
                if (companionId == _userId)
                    return Result.Failed<ChannelOutputModel>("Нельзя создать личную беседу с самим собой");
                var companionUserName = await _authApiClient.GetUserName(companionId);
                if (!companionUserName)
                    return Result.NotFound<ChannelOutputModel>($"Не найден пользователь с Id={companionId}");

                var alreadyExistingPrivateChannelId = await _repository
                    .Where(x =>
                        x.Type == ChannelType.PRIVATE &&
                        x.Users.Count() == 2 &&
                        x.Users.Select(x => x.UserId).All(userId => new[] { _userId, companionId }.Contains(userId)))
                    .Select(x => (int?)x.Id)
                    .FirstOrDefaultAsync();

                if (alreadyExistingPrivateChannelId != null)
                    return Result.Failed<ChannelOutputModel>($"С пользователем {companionUserName} уже есть существующая личная беседа Id={alreadyExistingPrivateChannelId}");

                model.UserIds.Add(_userId);
                return await CreateChannelInternal(model);
            }
            else if (model.Type == ChannelType.GROUP)
            {
                model.UserIds = model.UserIds.Distinct().ToList();

                var notExistingUsers = (await Task.WhenAll(model.UserIds.Select(async (userId) => new { Result = await _authApiClient.IsUserExists(userId), UserId = userId })))
                    .Where(x => !x.Result)
                    .Select(x => x.UserId)
                    .ToArray();

                if (notExistingUsers.Any())
                    return Result.NotFound<ChannelOutputModel>($"Не найдены пользователи Id=[{string.Join(", ", notExistingUsers)}]");

                if (model.Name.IsNullOrEmpty())
                    return Result.Failed<ChannelOutputModel>("Беседа должна иметь название");

                model.UserIds.Add(_userId);
                return await CreateChannelInternal(model);
            }

            throw new ArgumentException(nameof(model.Type));
        }

        private async Task<Result<ChannelOutputModel>> CreateChannelInternal(ChannelCreateModel model)
        {
            var channel = model.Adapt<Channel>();
            channel.Name ??= string.Empty;
            channel.CreateUserId = _userId;
            channel.Users = model.UserIds
                .Distinct()
                .Select(userId => new ChannelUser
                {
                    Channel = channel,
                    UserId = userId
                })
                .ToList();

            _context.Set<Channel>().Add(channel);
            await _context.SaveChangesAsync();

            var result = channel.Adapt<ChannelOutputModel>();
            result.CreateUserName = (await _authApiClient.GetUserName(result.CreateUserId)).GetObjectOrDefault(UserConstants.UndefinedUserName);

            if (channel.Type == ChannelType.PRIVATE)
            {
                var companionId = model.UserIds.Except(_userId.ToEnumerable()).First();
                result.Name = (await _authApiClient.GetUserName(companionId)).GetObjectOrDefault(UserConstants.UndefinedUserName);
            }

            return result;
        }

        public async Task<Result<MessageCorrelationOutputModel>> SendMessage(int channelId, SendMessageModel model)
        {
            var channel = await _repository
                .WithId(channelId)
                .Select(x => new
                {
                    Recievers = x.Users
                        .Select(x => new
                        {
                            x.UserId,
                            x.Silenced
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (channel == null)
            {
                return Result.NotFound<MessageCorrelationOutputModel>();
            }

            var createMessageResult = await _messageService.CreateMessage(channelId, model, channel.Recievers.Select(x => x.UserId).ToArray());

            if (createMessageResult)
            {
                // При отправке все прошлые входящие сообщения помечаются пришедшими и прочитанными
                await ReceiveMessages(channelId);
                await ReadMessages(channelId);

                var result = createMessageResult.Object.Adapt<MessageCorrelationOutputModel>();
                result.CorrelationId = model.CorrelationId;
                result.SendUserName = (await _authApiClient.GetUserName(result.SendUserId)).GetObjectOrDefault(UserConstants.UndefinedUserName);

                var recievers = channel.Recievers.DistinctBy(x => x.UserId).Select(x => x.UserId).ToArray();

                await _hubContext.Clients.Clients(_userHubRepository.GetUsersConnections(recievers)).ReceiveMessage(result);

                return Result.Success(result);
            }

            return createMessageResult.AsResult<MessageCorrelationOutputModel>();
        }

        public async Task ReadMessages(int channelId)
        {
            var batchSize = 1000;

            var processedCount = 0;
            do
            {
                var recievers = (await _repository.WithId(channelId).SelectMany(x => x.Users).Select(x => x.UserId).ToArrayAsync())
                    .Except(_userId.ToEnumerable())
                    .ToArray();

                var guids = await _context.Set<MessageUser>()
                    .Where(x => x.Message.ChannelId == channelId && x.Message.SendUserId != _userId && x.UserId == _userId && !x.Read)
                    .Select(x => x.MessageGuid)
                    .Take(batchSize)
                    .ToArrayAsync();

                await _context.Set<MessageUser>()
                    .Where(x => guids.Contains(x.MessageGuid))
                    .UpdateFromQueryAsync(new Dictionary<string, object>
                    {
                        { nameof(MessageUser.Read), true },
                        { nameof(MessageUser.ReadDate), DateTime.UtcNow },
                    });

                processedCount = guids.Length;

                if (recievers.Any()) 
                {
                    var connections = _userHubRepository.GetUsersConnections(recievers);

                    if (connections.Any())
                    {
                        foreach (var guid in guids)
                        {
                            await _hubContext.Clients.Clients(connections)
                                .MarkReadMessage(new MessageActionModel
                                {
                                    Guid = guid,
                                    ChannelId = channelId,
                                    ActorId = _userId
                                });
                        }
                    }
                }
            } while (processedCount > 0);
        }

        public async Task ReceiveMessages(int channelId)
        {
            var batchSize = 1000;

            var processedCount = 0;
            do
            {
                var recievers = (await _repository.WithId(channelId).SelectMany(x => x.Users).Select(x => x.UserId).ToArrayAsync())
                    .Except(_userId.ToEnumerable())
                    .ToArray();

                var guids = await _context.Set<MessageUser>()
                    .Where(x => x.Message.ChannelId == channelId && x.Message.SendUserId != _userId && x.UserId == _userId && !x.Received)
                    .Select(x => x.MessageGuid)
                    .Take(batchSize)
                    .ToArrayAsync();

                await _context.Set<MessageUser>()
                    .Where(x => guids.Contains(x.MessageGuid))
                    .UpdateFromQueryAsync(new Dictionary<string, object>
                    {
                        { nameof(MessageUser.Received), true },
                        { nameof(MessageUser.ReceivedDate), DateTime.UtcNow },
                    });

                processedCount = guids.Length;

                if (recievers.Any())
                {
                    var connections = _userHubRepository.GetUsersConnections(recievers);

                    if (connections.Any())
                    {
                        foreach (var guid in guids)
                        {
                            await _hubContext.Clients.Clients(connections)
                                .MarkReceivedMessage(new MessageActionModel
                                {
                                    Guid = guid,
                                    ChannelId = channelId,
                                    ActorId = _userId
                                });
                        }
                    }
                }
            } while (processedCount > 0);
        }
    }
}
