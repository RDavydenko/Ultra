
using Ultra.Msg.WebApi.Models.Message;

namespace Ultra.Msg.WebApi.Hubs.Interfaces
{
    public interface IChatHub
    {
        Task SendMessage(MessageOutputModel model, int[] recievers);
    }
}
