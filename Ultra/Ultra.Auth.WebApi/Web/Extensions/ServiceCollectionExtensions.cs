using Mapster;
using System.Reflection;

namespace Ultra.Auth.WebApi.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMapster(this IServiceCollection services)
        {
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
