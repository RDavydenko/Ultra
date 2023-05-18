using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ultra.Core.Entities.Attributes;
using Ultra.Core.Entities.Attributes.Access.Abstract;
using Ultra.Core.Entities.Attributes.Data.Methods.Abstract;
using Ultra.Core.Entities.Attributes.Meta.Abstract;
using Ultra.Core.Entities.Extensions;
using Ultra.Core.Entities.Interfaces;
using Ultra.Core.Extensions;
using Ultra.Core.Models;
using Ultra.Core.Models.Enums;
using Ultra.Core.Models.Meta;
using Ultra.Core.Services.Providers.Abstract;
using Ultra.Infrastructure.Models.Enums;

namespace Ultra.Core.Services.Providers
{
    public class EntityProvider : IEntityProvider
    {
        public async Task<string?> GetEntityDisplayName(Type entityType)
        {
            return entityType.GetCustomAttribute<DisplayAttribute>()?.DisplayName;
        }

        public async Task<string?> GetEntityFieldDisplayName(Type entityType, PropertyInfo property)
        {
            return property.GetCustomAttribute<DisplayAttribute>()?.DisplayName;
        }

        public async Task<EntityMethod[]> GetEntityMethods(Type entityType)
        {
            return entityType.GetCustomAttributes<AccessAttributeBase>(true).Where(x => x.IsAvailable).Select(x => x.Method).ToArray();
        }

        public async Task<FieldMethods[]> GetEntityFieldMethods(Type entityType, PropertyInfo property)
        {
            return property.GetCustomAttributes<MethodAttributeBase>(true).Where(x => x.IsAvailable).Select(x => x.Method).ToArray();
        }

        public async Task<Dictionary<string, object>> GetEntityFieldsMetadata(Type entityType, PropertyInfo property)
        {
            return property.GetCustomAttributes<MetaAttributeBase>()
                .SelectMany(x => x.GetMeta())
                .Concat(GetAdditionalMeta(entityType, property))
                .GroupBy(x => x.Key)
                .Select(g => g.First())
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public async Task<Dictionary<string, object>> GetEntityMetadata(Type entityType)
        {
            var dct = new Dictionary<string, object>();
            if (entityType.IsDisplayableEntity(out var displayFieldName))
            {
                dct.Add("displayable", displayFieldName);
            }
            return dct;
        }

        private IEnumerable<MetaModel> GetAdditionalMeta(Type entityType, PropertyInfo property)
        {
            if (property.GetCustomAttribute<ForeignKeyAttribute>() != null)
            {
                var attr = property.GetCustomAttribute<ForeignKeyAttribute>()!;
                var fkProperty = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .First(x => x.Name == attr.Name);
                var fkPropertyType = fkProperty.PropertyType;
                yield return new("foreignKey", true);
                yield return new("foreignKey.type", fkPropertyType.Name);
                if (fkPropertyType.IsDisplayableEntity(out var displayNameField))
                {
                    yield return new("foreignKey.displayable", displayNameField);
                }
            }

            // Reference Types
            var propertyType = property.PropertyType;
            if (IsReferenceChildren(entityType, propertyType))
            {
                // TODO: Надо переделывать
                var childType = propertyType.GetCollectionType();
                var fkProperty = childType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => 
                    {
                        var fkAttr = x.GetCustomAttribute<ForeignKeyAttribute>();
                        if (fkAttr == null) return false;

                        var fkProp = childType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .FirstOrDefault(p => p.Name == fkAttr.Name);

                        return fkProp != null && fkProp.PropertyType == entityType;
                    })
                    .First();
                var fkPropertyPath = fkProperty.GetCustomAttribute<ForeignKeyAttribute>()!.Name;
                var fkPropertyType = fkProperty.PropertyType;
                yield return new("foreignKey.path", fkProperty.Name);
                yield return new("foreignKey.type", childType.Name);

                if (childType.IsDisplayableEntity(out var displayNameField))
                {
                    yield return new("foreignKey.displayable", displayNameField);
                }
            }
            if (IsReferenceChild(entityType, propertyType))
            {
                // TODO
            }
            if (IsReferenceParent(entityType, propertyType))
            {
                var fkProperty = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => x.GetCustomAttribute<ForeignKeyAttribute>() != null)
                    .First();
                var fkPropertyPath = fkProperty.GetCustomAttribute<ForeignKeyAttribute>()!.Name;
                var fkPropertyType = fkProperty.PropertyType;
                yield return new("foreignKey.path", fkProperty.Name);
                yield return new("foreignKey.type", propertyType.Name);

                if (propertyType.IsDisplayableEntity(out var displayNameField))
                {
                    yield return new("foreignKey.displayable", displayNameField);
                }
            }
        }

        public async Task<FieldType> GetFieldType(Type entityType, PropertyInfo property)
        {
            return GetFieldTypeInternal(entityType, property);
        }

        private bool IsType<T>(Type propertyType)
        {
            return propertyType == typeof(T) || propertyType.IsNullableOf<T>();
        }

        private FieldType GetFieldTypeInternal(Type entityType, PropertyInfo property)
        {
            var propertyType = property.PropertyType;

            if (IsType<bool>(propertyType)) return FieldType.Boolean;
            if (IsType<sbyte>(propertyType)) return FieldType.Int8;
            if (IsType<byte>(propertyType)) return FieldType.UInt8;
            if (IsType<short>(propertyType)) return FieldType.Int16;
            if (IsType<ushort>(propertyType)) return FieldType.UInt16;
            if (IsType<int>(propertyType))
            {
                if (property.GetCustomAttribute<UserIdAttribute>() != null)
                {
                    return FieldType.UserId;
                }
                return FieldType.Int32;
            }
            if (IsType<uint>(propertyType)) return FieldType.UInt32;
            if (IsType<long>(propertyType)) return FieldType.Int64;
            if (IsType<ulong>(propertyType)) return FieldType.UInt64;
            if (IsType<decimal>(propertyType)) return FieldType.Decimal;
            if (IsType<float>(propertyType)) return FieldType.Float;
            if (IsType<double>(propertyType)) return FieldType.Double;
            if (IsType<char>(propertyType)) return FieldType.Char;
            if (IsType<string>(propertyType)) return FieldType.String;
            if (IsType<DateTime>(propertyType)) return FieldType.DateTime;
            if (IsType<DateTime>(propertyType)) return FieldType.Date; // + attribute
            if (IsType<DateTime>(propertyType)) return FieldType.Time; // TODO: + attribute
            if (propertyType == typeof(bool)) return FieldType.Interval; // TODO
            if (propertyType == typeof(bool)) return FieldType.DateTimePeriod; // TODO
            if (propertyType == typeof(bool)) return FieldType.DatePeriod;
            if (propertyType == typeof(bool)) return FieldType.TimePeriod;
            if (IsLocation(propertyType)) return FieldType.Location;
            if (propertyType == typeof(bool)) return FieldType.File;

            // TODO: inject other custom types

            // Reference Types
            if (IsReferenceChildren(entityType, propertyType)) return FieldType.ReferenceChildren;
            if (IsReferenceChild(entityType, propertyType)) return FieldType.ReferenceChild;
            if (IsReferenceParent(entityType, propertyType)) return FieldType.ReferenceParent;


            return FieldType.Undefined;
        }

        private bool IsLocation(Type propertyType)
        {
            return propertyType.IsGeoType();
        }

        private bool IsReferenceChildren(Type entityType, Type propertyType)
        {
            if (propertyType.IsCollectionOfDbEntities())
            {
                return true;
            }

            return false;
        }

        private bool IsReferenceParent(Type entityType, Type propertyType)
        {
            if (propertyType.IsClass && 
                propertyType.GetProperties()
                    .Any(x => x.PropertyType
                            .IsAssignableTo(typeof(IEnumerable<>).MakeGenericType(entityType))))
            {
                return true;
            }

            return false;
        }

        private bool IsReferenceChild(Type entityType, Type propertyType)
        {
            // TODO: Связь 1 к 1. Надо смотреть - на чьей стороне FK.
            // У кого FK - тот и родитель, а другой значит ребенок
            return false;
        }
    }
}
