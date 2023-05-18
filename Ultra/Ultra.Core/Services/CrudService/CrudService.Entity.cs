using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Ultra.Core.DAL;
using Ultra.Core.Entities.Extensions;
using Ultra.Core.Entities.Interfaces;
using Ultra.Core.Models;
using Ultra.Core.Services.Abstract;
using Ultra.Core.Services.CrudService.Abstract;
using Ultra.Core.Extensions;
using Ultra.Extensions;
using Microsoft.Linq.Translations;
using DelegateDecompiler;
using DelegateDecompiler.EntityFrameworkCore;
using System.Reflection;
using System;
using System.Collections.Generic;
using IdentityModel.Client;
using Ultra.Core.Tools;

namespace Ultra.Core.Services.CrudService
{
    public class CrudService<T> : CrudServiceBase<T>
        where T : class, IEntity
    {
        private readonly UltraDbContextBase _context;

        public CrudService(
            UltraDbContextBase context,
            IPermissionService<T> permissionService)
            : base(permissionService)
        {
            _context = context;
        }

        protected override Task<IQueryable<T>> _GetQueryable()
        {
            return _context.Set<T>().AsQueryable().AsTask();
        }

        protected override async Task<T> _CreateAsync(
            T createModel, RelatedLink[] linksToAddOrUpdate, RelatedLink[] linksToDelete)
        {
            await BeforeCreate(createModel);

            createModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsCollectionOfDbEntities())
                .ForEach(p => p.SetValue(createModel, null));

            _context.Set<T>().Add(createModel);
            await _context.SaveChangesAsync();

            await ResolveRelatedLinks(createModel.Id, linksToAddOrUpdate, linksToDelete);

            return createModel;
        }
        protected virtual Task BeforeCreate(T model) => Task.CompletedTask;

        protected override async Task<T> _UpdateAsync(
            int id, T existedEntity, T updateModel, RelatedLink[] linksToAddOrUpdate, RelatedLink[] linksToDelete)
        {
            var props = updateModel
                .ToDictionary(canRead: true, canWrite: true)
                .Where(x => typeof(T).GetProperty(x.Key)!.IsAvailableToUpdated());
            foreach (var prop in props)
            {
                var type = typeof(T).GetProperty(prop.Key)?.PropertyType;
                if (type == null || type.IsCollectionOfDbEntities())
                {
                    continue;
                }

                existedEntity.SetProperty(prop.Key, prop.Value);
            }

            await BeforeUpdate(id, existedEntity);

            _context.Set<T>().Update(existedEntity);
            await _context.SaveChangesAsync();

            await ResolveRelatedLinks(id, linksToAddOrUpdate, linksToDelete);

            return existedEntity;
        }

        protected virtual Task BeforeUpdate(int id, T model) => Task.CompletedTask;

        protected override async Task<T> _PatchAsync(int id, PatchModel<T> model)
        {
            await BeforePatch(id, model);

            await _context.Set<T>()
                .Where(x => x.Id == id)
                .UpdateFromQueryAsync(model.GetValues());

            var entity = await _context.Set<T>().FirstAsync(x => x.Id == id);

            return entity;
        }

        protected virtual Task BeforePatch(int id, PatchModel<T> model) => Task.CompletedTask;

        protected override async Task _DeleteAsync(int id)
        {
            await _context.Set<T>()
                .Where(x => x.Id == id)
                .DeleteFromQueryAsync();
        }

        private async Task ResolveRelatedLinks(int id, RelatedLink[] linksToAddOrUpdate, RelatedLink[] linksToDelete)
        {
            var sysNameToEntityDbTypeMap = new Dictionary<string, Type>();

            bool TryGetEntityType(string sysName, out Type entityType)
            {
                if (!sysNameToEntityDbTypeMap.TryGetValue(sysName, out entityType))
                {
                    entityType = Executor.WebAssembly.GetDbEntities().FirstOrDefault(x => x.Name == sysName);
                    if (entityType == null)
                    {
                        return false;
                    }
                    sysNameToEntityDbTypeMap.Add(sysName, entityType);
                }

                return true;
            }

            foreach (var link in linksToAddOrUpdate)
            {
                var sysName = link.EntitySystemName;
                if (!TryGetEntityType(sysName, out var entityType))
                {
                    continue;
                }

                await UpdatePropertyAsync(entityType, link.EntityId, link.EntityPropertyName, id);
            }

            foreach (var link in linksToDelete)
            {
                var sysName = link.EntitySystemName;
                if (!TryGetEntityType(sysName, out var entityType))
                {
                    continue;
                }

                await UpdatePropertyAsync(entityType, link.EntityId, link.EntityPropertyName, null);
            }
        }

        private async Task UpdatePropertyAsync<TEntity>(int id, string propertyName, object value)
            where TEntity : class, IEntity
        {
            await _context.Set<TEntity>()
                .Where(x => x.Id == id)
                .UpdateFromQueryAsync(new Dictionary<string, object>() { { propertyName, value } });
        }

        private async Task UpdatePropertyAsync(Type entityType, int id, string propertyName, object value)
        {
            await (Task)(typeof(CrudService<T>).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(m => m.Name == nameof(UpdatePropertyAsync) && m.IsGenericMethod && m.GetParameters().Length == 3)
                .MakeGenericMethod(entityType)
                .Invoke(this, new[] { id, propertyName, value })!);
        }
    }
}
