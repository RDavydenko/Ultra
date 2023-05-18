using HotChocolate.Types.Pagination;
using LinqKit;
using System.Linq;
using System.Threading.Tasks;
using Ultra.Core.Entities.Interfaces;
using Ultra.Core.Models.Enums;
using Ultra.Core.Services.Abstract;
using Ultra.Infrastructure.Http.Interfaces;
using Ultra.Infrastructure.Models;
using Ultra.Infrastructure.Models.Enums;

namespace Ultra.Core.Services
{
    public class PermissionService<T> : IPermissionService<T>
        where T : class, IEntity
    {
        private readonly IUserAccessor _userAccessor;
        private readonly IAuthApiClient _authApiClient;

        public PermissionService(
            IUserAccessor userAccessor,
            IAuthApiClient authApiClient)
        {
            _userAccessor = userAccessor;
            _authApiClient = authApiClient;
        }

        public Task<Result> CheckPermission(EntityMethod method, int id)
        {
            if (!_userAccessor.IsAuthenticated)
            {
                return Result.Failed("Пользователь не авторизован");
            }

            var userId = _userAccessor.UserId;
            return _authApiClient.CanEntityAccessByMethod(GetSystemName(), id, method, userId);
        }

        public Task<Result> CheckPermission(EntityMethod method)
        {
            if (!_userAccessor.IsAuthenticated)
            {
                return Result.Failed("Пользователь не авторизован");
            }

            var userId = _userAccessor.UserId;
            return _authApiClient.CanEntitiesAccessByMethod(GetSystemName(), method, userId);
        }

        public async Task<IQueryable<T>> PatchQueryable(IQueryable<T> queryable)
        {
            if (!_userAccessor.IsAuthenticated)
            {
                return queryable.Where(x => false);
            }

            var userId = _userAccessor.UserId;
            var result = await _authApiClient.GetViewPermissionModel(GetSystemName(), userId);
            if (!result)
            {
                return queryable.Where(x => false);
            }

            var model = result.Object;
            if (model.AllAccess)
            {
                return queryable.Where(x => !model.ExcludeIds.Contains(x.Id));
            }
            else if (model.AnyAccess)
            {
                return queryable
                    .Where(x => model.IncludeIds.Contains(x.Id) &&
                               !model.ExcludeIds.Contains(x.Id));
            }
            else
            {
                return queryable.Where(x => false);
            }
        }

        private string GetSystemName() => typeof(T).Name;
    }
}
