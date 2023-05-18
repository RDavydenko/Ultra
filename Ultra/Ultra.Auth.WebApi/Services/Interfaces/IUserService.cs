using Ultra.Auth.WebApi.Models;
using Ultra.Infrastructure.Models;

namespace Ultra.Auth.WebApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserModel>> GetCurrentUser();
        Task<Result<UserModel>> GetUser(int userId);
        Task<Result> ExistsUser(int userId);
        Task<Result<UserModel>> GetUser(string login);
        Task<Result<string>> GetUserName(int userId);
        Task<Result<List<RoleModel>>> GetUserRoles(int userId);
        Task<Result<CollectionPage<UserShortModel>>> SearchUsers(string? userName, int? userId);
    }
}
