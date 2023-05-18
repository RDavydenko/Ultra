namespace Ultra.Msg.WebApi.Models.Message
{
    public class MessageActionModel
    {
        public Guid Guid { get; set; }
        public int ChannelId { get; set; }
        public int ActorId { get; set; }
    }
}
