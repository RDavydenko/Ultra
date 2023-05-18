using System.Text.Json.Serialization;
using Ultra.Core.Entities.Abstract;

namespace Ultra.Msg.WebApi.DAL.Entities
{
    public class Channel : EntityBase
    {
        public string Name { get; set; }
        public ChannelType Type { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public int CreateUserId { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<ChannelUser> Users { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChannelType
    {
        PRIVATE,
        GROUP
    }
}
