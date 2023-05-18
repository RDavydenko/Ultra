using HotChocolate.Types;
using System;
using System.Linq;
using System.Reflection;

namespace Ultra.Core.Web.GraphQL.DistinctBy
{
    public class ObjectPropertyType<T> : EnumType<string>
    {
        public override TypeKind Kind => TypeKind.Enum;

        protected override void Configure(IEnumTypeDescriptor<string> descriptor)
        {
            var type = typeof(T);
            var properties = type.GetMembers().OfType<PropertyInfo>().Where(info => IsSimple(info.PropertyType));

            descriptor.Name(type.Name + "Property");
            descriptor.BindValuesExplicitly();

            foreach (var property in properties)
            {
                var name = property.Name;
                descriptor
                    .Value(name)
                    .Name(FirstCharToLower(name))
                    .Description(name);
            }
        }

        private string FirstCharToLower(string value) => value.Length > 1 ? $"{char.ToLower(value[0])}{value[1..]}" : value.ToLower();

        private bool IsSimple(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return IsSimple(type.GetGenericArguments()[0]);
            }

            if (type.IsPrimitive
                || type.IsEnum
                || type.Equals(typeof(string))
                || type.Equals(typeof(decimal))
                || type.Equals(typeof(DateTime)))
            {
                return true;
            }

            return false;
        }
    }
}
