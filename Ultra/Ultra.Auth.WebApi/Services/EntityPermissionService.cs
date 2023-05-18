using Microsoft.EntityFrameworkCore;
using Ultra.Auth.WebApi.DAL;
using Ultra.Auth.WebApi.DAL.Entities;
using Ultra.Auth.WebApi.Models;
using Ultra.Auth.WebApi.Services.Interfaces;
using Ultra.Core.Entities.States;
using Ultra.Core.Models.Enums;
using Ultra.Core.Services.Abstract;
using Ultra.Extensions;
using Ultra.Infrastructure.Models;
using Ultra.Infrastructure.Models.Enums;

namespace Ultra.Auth.WebApi.Services
{
    public class EntityPermissionService : IEntityPermissionService
    {
        private readonly AuthDbContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly IUserService _userService;

        public EntityPermissionService(
            AuthDbContext context,
            IUserAccessor userAccessor,
            IUserService userService)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userService = userService;
        }

        private async Task<IQueryable<EntityAccess>> GetUserEntityAccessItems(string systemName, int userId)
        {
            var userRolesIds = (await _userService.GetUserRoles(userId))
                .GetObjectOrThrow()
                .Where(x => x.State == State.ACTIVE)
                .Select(x => x.Id)
                .ToList();

            return _context.Set<EntityAccess>()
                .AsNoTracking()
                .Where(x => x.Entity == systemName &&
                           (x.UserId == userId || (x.RoleId != null && userRolesIds.Contains(x.RoleId.Value))));
        }

        public async Task<Result<EntityPermissionModel>> GetViewPermissionModel(string systemName, int userId)
        {
            var entityAccessItems = await (await GetUserEntityAccessItems(systemName, userId))
                .Where(x => x.Method == EntityMethod.Read)
                .Select(x => new { x.Type, x.EntityId })
                .ToListAsync();

            // Есть запись с белым списком ИЛИ
            // Есть нет записи с черным списком на все сущности
            var blackListOnEntityType = entityAccessItems.Any(x => x.EntityId == null && x.Type == AccessType.B);
            var hasAccess = entityAccessItems.Any(x => x.Type == AccessType.W) || !blackListOnEntityType;
            var model = new EntityPermissionModel
            {
                AnyAccess = hasAccess,
                AllAccess = !blackListOnEntityType,
                IncludeIds = entityAccessItems.Where(x => x.Type == AccessType.W).SelectNotNull(x => x.EntityId).ToArray(),
                ExcludeIds = entityAccessItems.Where(x => x.Type == AccessType.B).SelectNotNull(x => x.EntityId).ToArray(),
            };

            return model;
        }

        public async Task<Result> CanEntityAccessByMethod(string systemName, int id, EntityMethod method, int userId)
        {
            var entityAccessItems = await (await GetUserEntityAccessItems(systemName, userId))
                .Where(x => x.EntityId == id && x.Method == method)
                .Select(x => new { x.Type })
                .ToListAsync();

            if (!entityAccessItems.Any(x => x.Type == AccessType.B))
            {
                return Result.Success();
            }

            return Result.Failed($"У пользователя нет доступа к {systemName} с Id={id}. Действие {method}");
        }

        public async Task<Result> CanEntityAccessByMethod(string systemName, EntityMethod method, int userId)
        {
            var entityAccessItems = await (await GetUserEntityAccessItems(systemName, userId))
                .Where(x => x.EntityId == null && x.Method == method)
                .Select(x => new { x.Type })
                .ToListAsync();

            if (!entityAccessItems.Any(x => x.Type == AccessType.B))
            {
                return Result.Success();
            }

            return Result.Failed($"У пользователя нет доступа к {systemName}. Действие {method}");
        }
    }
}
