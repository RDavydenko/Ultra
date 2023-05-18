using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Ultra.Core.Models.Enums;
using Ultra.Infrastructure.Models.Enums;

namespace Ultra.Core.Services.Providers.Abstract
{
    public interface IEntityProvider
    {
        Task<string?> GetEntityDisplayName(Type entityType);
        Task<string?> GetEntityFieldDisplayName(Type entityType, PropertyInfo property);
        Task<FieldMethods[]> GetEntityFieldMethods(Type entityType, PropertyInfo property);
        Task<Dictionary<string, object>> GetEntityFieldsMetadata(Type entityType, PropertyInfo property);
        Task<Dictionary<string, object>> GetEntityMetadata(Type entityType);
        Task<EntityMethod[]> GetEntityMethods(Type entityType);
        Task<FieldType> GetFieldType(Type entityType, PropertyInfo property);
    }
}