using Ultra.Infrastructure.Http.Abstract;
using Ultra.Infrastructure.Http.Interfaces;
using Ultra.Infrastructure.Models;
using Ultra.Infrastructure.Models.Enums;

namespace Ultra.Infrastructure.Http
{
    public class AuthApiClient : IAuthApiClient
    {
        private readonly AuthorizedHttpClientBase _client;

        public AuthApiClient(AuthorizedHttpClientBase client)
        {
            _client = client;
        }

        public Task<Result> CanEntityAccessByMethod(string systemName, int id, EntityMethod method, int userId)
            => _client.GetAsync<Result>($"access/{systemName}/entity/{id}/{method}?userId={userId}");

        public Task<Result> CanEntitiesAccessByMethod(string systemName, EntityMethod method, int userId)
            => _client.GetAsync<Result>($"access/{systemName}/entities/{method}?userId={userId}");

        public Task<Result<EntityPermissionModel>> GetViewPermissionModel(string systemName, int userId) 
            => _client.GetResultAsync<EntityPermissionModel>($"access/{systemName}/view?userId={userId}");

        public Task<Result<string>> GetUserName(int userId)
            => _client.GetResultAsync<string>($"users/{userId}/userName");

        public Task<Result> IsUserExists(int userId)
            => _client.GetResultAsync($"users/{userId}/exists");
    }
}
