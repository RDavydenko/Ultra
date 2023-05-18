namespace Ultra.Msg.WebApi.DAL.Entities
{
    public class ChannelUser
    {
        public int ChannelId { get; set; }
        public virtual Channel Channel { get; set; }
        public int UserId { get; set; }
        public bool Silenced { get; set; }
    }
}
