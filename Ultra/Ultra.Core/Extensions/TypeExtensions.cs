using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ultra.Core.Entities.Interfaces;

namespace Ultra.Core.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsReferenceType(this Type type) => type.IsClass;

        public static bool IsCollectionType(this Type type) => typeof(IEnumerable).IsAssignableFrom(type);

        public static Type GetCollectionType(this Type type) =>
            type.GetInterfaces()
                .First(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .GetGenericArguments()[0];

        public static bool IsCollectionOfDbEntities(this Type type) =>
            IsCollectionType(type) &&
            IsDbEntityType(GetCollectionType(type));

        public static bool IsDbEntityType(this Type type) => typeof(IDbEntity).IsAssignableFrom(type);

        public static bool IsGeoEntity(this Type type) => type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Any(p => p.PropertyType.IsGeoType());

        public static bool IsGeoType(this Type type)
        {
            var locationTypes = new[] { typeof(NetTopologySuite.Geometries.Point) };
            return locationTypes.Contains(type);
        }
    }
}
