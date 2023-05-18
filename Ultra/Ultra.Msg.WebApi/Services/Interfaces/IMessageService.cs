using Ultra.Infrastructure.Models;
using Ultra.Msg.WebApi.DAL.Entities;
using Ultra.Msg.WebApi.Models.Message;

namespace Ultra.Msg.WebApi.Services.Interfaces
{
    public interface IMessageService
    {
        Task<Result<Message>> CreateMessage(int channelId, SendMessageModel model, int[] recieverIds);
        Task ReadMessage(Guid messageGuid);
        Task ReceiveMessage(Guid messageGuid);
    }
}
