using Ultra.Infrastructure.Models;
using Ultra.Msg.WebApi.Models.Channel;
using Ultra.Msg.WebApi.Models.Message;

namespace Ultra.Msg.WebApi.Services.Interfaces
{
    public interface IChannelService
    {
        Task<Result<CollectionPage<ChannelFullOutputModel>>> GetChannels(PageModel? pageModel = null);
        Task<Result<ChannelFullOutputModel>> GetChannel(int channelId);
        Task<Result<ChannelOutputModel>> CreateChannel(ChannelCreateModel model);
        Task<Result<CollectionPage<MessageFullOutputModel>>> GetMessages(int channelId, PageModel? model = null);
        Task<Result<int>> GetUnreadMessagesCount();
        Task<Result<MessageCorrelationOutputModel>> SendMessage(int channelId, SendMessageModel model);
        Task ReadMessages(int channelId);
        Task ReceiveMessages(int channelId);
    }
}
