namespace Ultra.Msg.WebApi.Models.Message
{
    public class MessageFullOutputModel : MessageOutputModel
    {
        public bool Received { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public bool Read { get; set; }
        public DateTime? ReadDate { get; set; }
    }
}
