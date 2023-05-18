using Ultra.Extensions;
using Ultra.Msg.WebApi.Hubs.Abstract;
using Ultra.Msg.WebApi.Hubs.Clients;
using Ultra.Msg.WebApi.Hubs.Repositories;

namespace Ultra.Msg.WebApi.Hubs
{
    public class ChatHub : AuthorizedHub<IChatClient>
    {
        public ChatHub(UserHubRepository repository)
            : base(repository)
        {
        }
    }
}
