using Ultra.Msg.WebApi.DAL.Entities;

namespace Ultra.Msg.WebApi.Models.Channel
{
    public class ChannelCreateModel
    {
        public ChannelType Type { get; set; }
        public string? Name { get; set; }
        public List<int> UserIds { get; set; } = new();
    }
}
