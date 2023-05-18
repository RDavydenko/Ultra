namespace Ultra.Msg.WebApi.Models.Message
{
    public class MessageOutputModel
    {
        public Guid Guid { get; set; }
        public string Text { get; set; }
        public int ChannelId { get; set; }
        public int SendUserId { get; set; }
        public string SendUserName { get; set; }
        public DateTime SendDate { get; set; }
    }
}
