using System.Collections.Concurrent;
using System.Security.Claims;
using Ultra.Core.Models.Auth;

namespace Ultra.Msg.WebApi.Hubs.Repositories
{
    public class UserHubRepository
    {
        protected ConcurrentDictionary<int, HashSet<string>> _userConnections = new();

        public void Connect(ClaimsPrincipal user, string connectionId)
        {
            var userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value);

            if (!_userConnections.TryGetValue(userId, out var connections))
            {
                connections = new();
                _userConnections[userId] = connections;
            }
            connections.Add(connectionId);
        }

        public void Disconnect(ClaimsPrincipal user, string connectionId)
        {
            var userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id).Value);

            if (_userConnections.TryGetValue(userId, out var connections))
            {
                connections.Remove(connectionId);
            }
        }

        public string[] GetUserConnections(int userId)
        {
            var connections = _userConnections.GetValueOrDefault(userId, new()).ToArray();
            return connections;
        }

        public string[] GetUsersConnections(int[] userIds) =>
            userIds.SelectMany(userId => GetUserConnections(userId)).ToArray();
    }
}
