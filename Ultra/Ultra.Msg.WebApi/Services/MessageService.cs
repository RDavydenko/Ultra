using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Ultra.Core.Services.Abstract;
using Ultra.Infrastructure.Models;
using Ultra.Msg.WebApi.DAL;
using Ultra.Msg.WebApi.DAL.Entities;
using Ultra.Msg.WebApi.Hubs.Clients;
using Ultra.Msg.WebApi.Hubs;
using Ultra.Msg.WebApi.Models.Message;
using Ultra.Msg.WebApi.Services.Interfaces;
using Ultra.Msg.WebApi.Hubs.Repositories;
using System.Threading.Channels;

namespace Ultra.Msg.WebApi.Services
{
    public class MessageService : IMessageService
    {
        private readonly MsgDbContext _context;
        private readonly IUserAccessor _accessor;
        private readonly IHubContext<ChatHub, IChatClient> _hubContext;
        private readonly UserHubRepository _userHubRepository;
        private readonly int _userId;

        public MessageService(
            MsgDbContext context,
            IUserAccessor accessor,
            IHubContext<ChatHub, IChatClient> hubContext,
            UserHubRepository userHubRepository)
        {
            _context = context;
            _accessor = accessor;
            _hubContext = hubContext;
            _userHubRepository = userHubRepository;
            _userId = _accessor.UserId;
        }

        public async Task<Result<Message>> CreateMessage(int channelId, SendMessageModel model, int[] recieverIds)
        {
            var msg = model.Adapt<Message>();

            var sendDate = msg.SendDate;

            msg.SendUserId = _userId;
            msg.ChannelId = channelId;
            msg.Users = recieverIds.Append(_userId)
                .Distinct()
                .Select(userId => new MessageUser
                {
                    Message = msg,
                    UserId = userId,
                })
                .ToList();

            _context.Set<Message>().Add(msg);

            await _context.SaveChangesAsync();

            return msg;
        }

        public async Task ReadMessage(Guid messageGuid)
        {
            var message = await _context.Set<MessageUser>()
                .Where(x => x.MessageGuid == messageGuid && x.Message.SendUserId != _userId && x.UserId == _userId && !x.Read)
                .Select(x => new { x.MessageGuid, x.Message.SendUserId, x.Message.ChannelId })
                .FirstOrDefaultAsync();

            if (message == null)
            {
                return;
            }

            await _context.Set<MessageUser>()
                .Where(x => x.MessageGuid == message.MessageGuid)
                .UpdateFromQueryAsync(new Dictionary<string, object>
                {
                    { nameof(MessageUser.Read), true },
                    { nameof(MessageUser.ReadDate), DateTime.UtcNow },
                });

            await _hubContext.Clients.Clients(_userHubRepository.GetUserConnections(message.SendUserId))
                .MarkReadMessage(new MessageActionModel
                {
                    Guid = messageGuid,
                    ChannelId = message.ChannelId,
                    ActorId = _userId
                });
        }

        public async Task ReceiveMessage(Guid messageGuid)
        {
            var message = await _context.Set<MessageUser>()
                .Where(x => x.MessageGuid == messageGuid && x.Message.SendUserId != _userId && x.UserId == _userId && !x.Received)
                .Select(x => new { x.MessageGuid, x.Message.SendUserId, x.Message.ChannelId })
                .FirstOrDefaultAsync();

            if (message == null)
            {
                return;
            }

            await _context.Set<MessageUser>()
                .Where(x => x.MessageGuid == message.MessageGuid)
                .UpdateFromQueryAsync(new Dictionary<string, object>
                {
                    { nameof(MessageUser.Received), true },
                    { nameof(MessageUser.ReceivedDate), DateTime.UtcNow },
                });

            await _hubContext.Clients.Clients(_userHubRepository.GetUserConnections(message.SendUserId))
                .MarkReceivedMessage(new MessageActionModel
                {
                    Guid = messageGuid,
                    ChannelId = message.ChannelId,
                    ActorId = _userId
                });
        }
    }
}
