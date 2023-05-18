using Ultra.Msg.WebApi.DAL.Entities;

namespace Ultra.Msg.WebApi.Models.Channel
{
    public class ChannelOutputModel
    {
        public int Id { get; set; }
        public ChannelType Type { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public int[] UserIds { get; set; }
    }
}
