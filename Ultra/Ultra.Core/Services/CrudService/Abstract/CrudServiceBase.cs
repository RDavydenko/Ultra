using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Ultra.Core.Entities.Interfaces;
using Ultra.Core.Extensions;
using Ultra.Core.Models;
using Ultra.Core.Models.Enums;
using Ultra.Core.Services.Abstract;
using Ultra.Extensions;
using Ultra.Infrastructure.Models;
using Ultra.Infrastructure.Models.Enums;

namespace Ultra.Core.Services.CrudService.Abstract
{
    public abstract class CrudServiceBase<T> : ICrudService<T>
        where T : class, IEntity
    {
        private readonly IPermissionService<T> _permissionService;

        public CrudServiceBase(
            IPermissionService<T> permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task<IQueryable<T>> GetQueryable()
        {   
            var queryable = await _GetQueryable();
            return await _permissionService.PatchQueryable(queryable);
        }

        protected abstract Task<IQueryable<T>> _GetQueryable();

        public async Task<Result<T>> GetAsync(int id)
        {
            if (await _permissionService.CheckPermission(EntityMethod.Read, id))
            {
                var queryable = await GetQueryable();
                var entity = await queryable.FirstOrDefaultAsync(x => x.Id == id);
                if (entity != null)
                {
                    return Result.Success(entity);
                }
            }

            return Result.Failed<T>();
        }

        public async Task<Result<T>> CreateAsync(
            T model, RelatedLink[]? linksToAddOrUpdate = null, RelatedLink[]? linksToDelete = null)
        {
            if (!await _permissionService.CheckPermission(EntityMethod.Create))
            {
                return Result.Failed<T>();
            }

            var entity = await _CreateAsync(
                model,
                linksToAddOrUpdate ?? Array.Empty<RelatedLink>(),
                linksToDelete ?? Array.Empty<RelatedLink>());

            return Result.Success(entity);
        }

        protected abstract Task<T> _CreateAsync(T createModel, RelatedLink[] linksToAddOrUpdate, RelatedLink[] linksToDelete);

        public async Task<Result<T>> UpdateAsync(
            int id, T model, RelatedLink[]? linksToAddOrUpdate = null, RelatedLink[]? linksToDelete = null)
        {
            if (!await _permissionService.CheckPermission(EntityMethod.Update, id))
            {
                return Result.Failed<T>();
            }

            var queryable = await GetQueryable();
            var entity = await queryable.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return Result.Failed<T>();
            }

            var updated = await _UpdateAsync(
                id, 
                entity, 
                model,
                linksToAddOrUpdate ?? Array.Empty<RelatedLink>(),
                linksToDelete ?? Array.Empty<RelatedLink>());

            return Result.Success(updated);
        }

        protected abstract Task<T> _UpdateAsync(int id, T existed, T updateModel, RelatedLink[] linksToAddOrUpdate, RelatedLink[] linksToDelete);

        public async Task<Result<T>> PatchAsync(int id, PatchModel<T> model)
        {
            if (!await _permissionService.CheckPermission(EntityMethod.Update, id))
            {
                return Result.Failed<T>();
            }

            var queryable = await GetQueryable();
            var entityExists = await queryable.AnyAsync(x => x.Id == id);
            if (!entityExists)
            {
                return Result.Failed<T>();
            }

            var entity = await _PatchAsync(id, model);

            return Result.Success(entity);
        }

        protected abstract Task<T> _PatchAsync(int id, PatchModel<T> patchModel);

        public async Task<Result> DeleteAsync(int id)
        {
            if (!await _permissionService.CheckPermission(EntityMethod.Delete, id))
            {
                return Result.Failed();
            }

            var queryable = await GetQueryable();
            var entityExists = await queryable.AnyAsync(x => x.Id == id);
            if (!entityExists)
            {
                return Result.Failed();
            }

            await _DeleteAsync(id);

            return Result.Success();
        }

        protected abstract Task _DeleteAsync(int id);
    }
}
