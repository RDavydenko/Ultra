using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Ultra.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static void SetProperty<T, E>(this T obj, Expression<Func<T, E>> propertyExpr, E value)
        {
            obj.SetProperty((propertyExpr.Body as MemberExpression).Member.Name, value);
        }

        public static void SetProperty<T, E>(this T obj, string name, E value)
        {
            obj.GetType().GetProperty(name).SetValue(obj, value);
        }

        public static T? GetAttribute<T>(this Type type)
            where T : Attribute
        {
            return type.GetCustomAttributes<T>().FirstOrDefault();
        }

        public static bool IsNullableOf<T>(this Type type)
        {
            return 
                Nullable.GetUnderlyingType(type) != null &&
                type.GenericTypeArguments.Length == 1 &&
                type.GenericTypeArguments[0] == typeof(T);
        }

        public static IDictionary<string, object> ToDictionary<T>(this T obj, bool canRead = true, bool canWrite = false)
        {
            var props = obj.GetType()
                .GetProperties()
                .Where(p => (p.CanRead || !canRead) && (p.CanWrite || !canWrite))
                .ToList();

            var res = new Dictionary<string, object>(props.Count);
            props.ForEach(p => res.Add(p.Name, p.GetValue(obj)));
            return res;
        }
    }
}
