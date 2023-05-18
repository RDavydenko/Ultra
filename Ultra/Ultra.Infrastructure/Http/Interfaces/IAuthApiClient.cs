using Ultra.Infrastructure.Models;
using Ultra.Infrastructure.Models.Enums;

namespace Ultra.Infrastructure.Http.Interfaces
{
    public interface IAuthApiClient
    {
        Task<Result<EntityPermissionModel>> GetViewPermissionModel(string systemName, int userId);
        Task<Result> CanEntitiesAccessByMethod(string systemName, EntityMethod method, int userId);
        Task<Result> CanEntityAccessByMethod(string systemName, int id, EntityMethod method, int userId);
        Task<Result<string>> GetUserName(int userId);
        Task<Result> IsUserExists(int userId);
    }
}
