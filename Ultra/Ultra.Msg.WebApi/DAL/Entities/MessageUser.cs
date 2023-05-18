namespace Ultra.Msg.WebApi.DAL.Entities
{
    public class MessageUser
    {
        public Guid MessageGuid { get; set; }
        public virtual Message Message { get; set; }
        public int UserId { get; set; }
        public bool Received { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public bool Read { get; set; }
        public DateTime? ReadDate { get; set; }
    }
}
