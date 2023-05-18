using Ultra.Auth.WebApi.Models;
using Ultra.Core.Models.Enums;
using Ultra.Infrastructure.Models;
using Ultra.Infrastructure.Models.Enums;

namespace Ultra.Auth.WebApi.Services.Interfaces
{
    public interface IEntityPermissionService
    {
        Task<Result<EntityPermissionModel>> GetViewPermissionModel(string systemName, int userId);
        Task<Result> CanEntityAccessByMethod(string systemName, EntityMethod method, int userId);
        Task<Result> CanEntityAccessByMethod(string systemName, int id, EntityMethod method, int userId);
    }
}
