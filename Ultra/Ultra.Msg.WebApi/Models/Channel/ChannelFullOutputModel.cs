using Ultra.Msg.WebApi.DAL.Entities;
using Ultra.Msg.WebApi.Models.Message;

namespace Ultra.Msg.WebApi.Models.Channel
{
    public class ChannelFullOutputModel : ChannelOutputModel
    {
        public bool Silenced { get; set; }
        public MessageFullOutputModel? LastMessage { get; set; }
        public int UnreadMessagesCount { get; set; }
    }
}
