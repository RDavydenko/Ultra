using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ultra.Core.DAL;
using Ultra.Core.DAL.Entities;
using Ultra.Core.DAL.Extensions;
using Ultra.Core.Entities.Extensions;
using Ultra.Core.Entities.Interfaces;
using Ultra.Core.Extensions;
using Ultra.Core.Models;
using Ultra.Core.Services.Abstract;
using Ultra.Core.Services.Providers;
using Ultra.Core.Services.Providers.Abstract;
using Ultra.Core.Tools;
using Ultra.Extensions;
using Ultra.Infrastructure.Models;

namespace Ultra.Core.Services
{
    public class EntityService : IEntityService
    {
        private readonly UltraDbContextBase _context;
        private readonly IUserAccessor _userAccessor;
        private readonly ICrudServiceHelper _crudServiceHelper;
        private readonly IEntityProvider _entityProvider;

        public EntityService(
            UltraDbContextBase context, 
            IUserAccessor userAccessor,
            ICrudServiceHelper crudServiceHelper,
            IEntityProvider entityProvider)
        {
            _context = context;
            _userAccessor = userAccessor;
            _crudServiceHelper = crudServiceHelper;
            _entityProvider = entityProvider;
        }

        public async Task<Result<EntityConfiguration>> GetConfiguration(string sysName)
        {
            var entityType = GetEntityType(sysName);
            if (entityType == null)
            {
                return Result.Failed<EntityConfiguration>("Определение сущности не найдено");
            }

            return new EntityConfiguration()
            {
                SystemName = sysName,
                DisplayName = await _entityProvider.GetEntityDisplayName(entityType),
                Methods = await _entityProvider.GetEntityMethods(entityType),
                Fields = await entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => !x.IsHidden())
                    .OrderByDescending(x => x.Name == nameof(IEntity.Id))
                    .SelectAsync(async x =>
                        new FieldConfiguration
                        {
                            SystemName = x.Name,
                            DisplayName = await _entityProvider.GetEntityFieldDisplayName(entityType, x),
                            Methods = await _entityProvider.GetEntityFieldMethods(entityType, x),
                            Type = await _entityProvider.GetFieldType(entityType, x),
                            Meta = await _entityProvider.GetEntityFieldsMetadata(entityType, x)
                        }
                    )
                    .ToListAsync(),
                Meta = await _entityProvider.GetEntityMetadata(entityType)
            };
        }

        private Type? GetEntityType(string sysName)
            => Executor.WebAssembly
                .GetDbEntities()
                .FirstOrDefault(x => x.Name == sysName);
        
        public async Task<Result> ToggleFavorite(string sysName, int? id = null)
        {
            var entityTypeId = await _context.Set<EntityType>()
                .Where(x => x.SystemName == sysName)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync();

            if (entityTypeId == null)
            {
                return Result.NotFound();
            }

            var userId = _userAccessor.UserId;
            var favorite = await _context.Set<FavoriteItem>()
                .Where(x => x.UserId == userId && x.EntityTypeId == entityTypeId.Value)
                .WhereIf(x => x.EntityId == id!.Value, id.HasValue)
                .FirstOrDefaultAsync();

            if (favorite == null)
            {
                _context.Set<FavoriteItem>()
                    .Add(new FavoriteItem
                    {
                        UserId = userId,
                        EntityTypeId = entityTypeId.Value,
                        EntityId = id
                    });
            }
            else
            {
                _context.Set<FavoriteItem>().Remove(favorite);
            }

            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<bool>> GetEntityFavorite(string sysName, int id)
        {
            var entityTypeId = await _context.Set<EntityType>()
                .Where(x => x.SystemName == sysName)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync();

            if (entityTypeId == null)
            {
                return Result.NotFound<bool>();
            }

            var userId = _userAccessor.UserId;
            var favorite = await _context.Set<FavoriteItem>()
                .Where(x => x.UserId == userId && x.EntityTypeId == entityTypeId.Value && x.EntityId == id)
                .AnyAsync();

            return Result.Success(favorite);
        }

        public async Task<Result<CollectionPage<EntityTypeOutputModel>>> GetEntityTypesPage(PageModel? pageModel = null)
        {
            //var r = new Random(1);
            //var total = 126;

            //var page = Enumerable.Range(0, total)
            //    .Select((x, i) => new EntityTypeOutputModel
            //    {
            //        Id = i + 1,
            //        SystemName = r.Next().ToString() + r.Next().ToString(),
            //        DisplayName = r.Next().ToString() + r.Next().ToString() + r.Next().ToString(),
            //        IsGeoEntity = r.Next(5) == 1,
            //        Favorite = r.Next(5) == 1,
            //        Count = r.Next(1000)
            //    })
            //    .AsQueryable()
            //    .Page(pageModel)
            //    .ToList();

            //return new CollectionPage<EntityTypeOutputModel>(page, new()
            //{
            //    Pages = Math.DivRem(total, pageModel?.PageSize == 0 ? 1 : pageModel?.PageSize ?? 1, out var res) + (res > 0 ? 1 : 0),
            //    TotalCount = total
            //});


            var userId = _userAccessor.UserId;
            var collection = await _context.Set<EntityType>()
                .AsNoTracking()
                .ProjectToType<EntityTypeOutputModel>()
                .ToCollectionPageAsync(pageModel);

            var entityTypeIds = collection.Items.Select(x => x.Id).ToList();

            // Проставляем Favorite
            (await _context.Set<FavoriteItem>()
                .Where(x => x.UserId == userId && x.EntityId == null && entityTypeIds.Contains(x.EntityTypeId))
                .Select(x => x.EntityTypeId)
                .ToListAsync())
            .ForEach(id => collection.Items.Where(x => x.Id == id).ForEach(x => x.Favorite = true));

            // Определяем количество записей
            var dbEntities = Executor.WebAssembly.GetDbEntities().ToList();
            foreach (var item in collection.Items)
            {
                var entityType = dbEntities.First(x => x.Name == item.SystemName);
                item.Count = await _crudServiceHelper.GetCountAsync(entityType);
                item.IsGeoEntity = entityType.IsGeoEntity();
            }

            return collection;
        }

        public async Task<Result<List<EntityTypeShortOutputModel>>> GetGeoEntityTypes()
        {
            var geoEntities = Executor.WebAssembly.GetDbEntities()
                .Where(x => x.IsGeoEntity())
                .Select(x => new { Type = x, SystemName = x.Name })
                .ToList();

            var dbEntities = await _context.Set<EntityType>()
                .Where(x => geoEntities.Select(x => x.SystemName).Contains(x.SystemName))
                .ProjectToType<EntityTypeShortOutputModel>()
                .ToListAsync();

            foreach (var item in dbEntities)
            {
                var geoEntity = geoEntities.First(x => x.SystemName == item.SystemName);
                if (geoEntity.Type.IsDisplayableEntity(out var displayFieldName))
                {
                    item.DisplayableField = displayFieldName;
                }
                item.GeoFieldName = geoEntity.Type
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .First(x => x.PropertyType.IsGeoType())
                    .Name;
            }

            return dbEntities;
        }
    }
}
