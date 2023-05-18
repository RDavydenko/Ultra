using Ultra.Msg.WebApi.Models.Message;

namespace Ultra.Msg.WebApi.Hubs.Clients
{
    public interface IChatClient
    {
        Task ReceiveMessage(MessageOutputModel message);
        Task MarkReadMessage(MessageActionModel message);
        Task MarkReceivedMessage(MessageActionModel message);
    }
}
