using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using Ultra.Msg.WebApi.Hubs.Repositories;

namespace Ultra.Msg.WebApi.Hubs.Abstract
{
    [Authorize]
    public abstract class AuthorizedHub<T> : Hub<T>
        where T : class
    {
        protected static ConcurrentDictionary<int, HashSet<string>> _userConnections = new();
        private readonly UserHubRepository _repository;

        public AuthorizedHub(UserHubRepository repository)
        {
            _repository = repository;
        }

        public override Task OnConnectedAsync()
        {
            _repository.Connect(Context.User, Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _repository.Disconnect(Context.User, Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        protected T GetUser(int userId)
        {
            var connections = _repository.GetUserConnections(userId);
            return Clients.Clients(connections);
        }
    }
}
