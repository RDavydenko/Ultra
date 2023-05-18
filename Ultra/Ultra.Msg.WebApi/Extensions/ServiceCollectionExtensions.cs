using Mapster;

namespace Ultra.Msg.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMapster(this IServiceCollection services)
        {
            TypeAdapterConfig.GlobalSettings.Scan(typeof(Program).Assembly);
            return services;
        }
    }
}
