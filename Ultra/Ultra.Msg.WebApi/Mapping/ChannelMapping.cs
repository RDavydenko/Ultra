using Mapster;
using Ultra.Msg.WebApi.DAL.Entities;
using Ultra.Msg.WebApi.Models.Channel;

namespace Ultra.Msg.WebApi.Mapping
{
    public class ChannelMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<Channel, ChannelOutputModel>()
                .Map(trg => trg.UserIds, src => src.Users.Select(x => x.UserId).ToArray());
        }
    }
}
