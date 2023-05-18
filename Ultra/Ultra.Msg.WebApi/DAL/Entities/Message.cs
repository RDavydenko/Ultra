using Ultra.Core.Entities.Abstract;

namespace Ultra.Msg.WebApi.DAL.Entities
{
    public class Message : GuidEntityBase
    {
        public int ChannelId { get; set; }
        public virtual Channel Channel { get; set; }
        public int SendUserId { get; set; }
        public DateTime SendDate { get; set; } = DateTime.UtcNow;
        public string Text { get; set; }

        public virtual ICollection<MessageUser> Users { get; set; }
    }
}
