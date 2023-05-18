using System.Reflection;
using Ultra.Core.Entities.Attributes.Data.Methods;
using Ultra.Core.Entities.Attributes.Data.Methods.Abstract;
using Ultra.Core.Entities.Attributes.Security;

namespace Ultra.Core.Entities.Extensions
{
    internal static class EntityPropertyExtensions
    {
        public static bool IsAvailableToCreated(this PropertyInfo prop) => prop.IsAvailableTo<CreatedAttribute>();
        public static bool IsAvailableToUpdated(this PropertyInfo prop) => prop.IsAvailableTo<UpdatedAttribute>();
        public static bool IsAvailableToPatched(this PropertyInfo prop) => prop.IsAvailableTo<PatchedAttribute>();

        public static bool IsHidden(this PropertyInfo prop) => prop.GetCustomAttribute<HiddenAttribute>()?.IsHidden == true;

        private static bool IsAvailableTo<TAttribute>(this PropertyInfo prop)
            where TAttribute : MethodAttributeBase
        {
            return prop.GetCustomAttribute<TAttribute>()?.IsAvailable == true;
        }
    }
}
