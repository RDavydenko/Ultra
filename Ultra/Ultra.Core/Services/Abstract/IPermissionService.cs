using System.Linq;
using System.Threading.Tasks;
using Ultra.Core.Entities.Interfaces;
using Ultra.Core.Models.Enums;
using Ultra.Infrastructure.Models;
using Ultra.Infrastructure.Models.Enums;

namespace Ultra.Core.Services.Abstract
{
    public interface IPermissionService<T>
        where T : class, IEntity
    {
        Task<Result> CheckPermission(EntityMethod method);
        Task<Result> CheckPermission(EntityMethod method, int id);
        Task<IQueryable<T>> PatchQueryable(IQueryable<T> queryable);
    }
}
