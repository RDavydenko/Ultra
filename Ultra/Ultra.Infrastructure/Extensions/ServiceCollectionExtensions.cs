using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ultra.Infrastructure.Http;
using Ultra.Infrastructure.Http.Abstract;
using Ultra.Infrastructure.Http.Interfaces;
using Ultra.Infrastructure.Models.Auth.Configuration;

namespace Ultra.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthApiClient(this IServiceCollection services, string serviceUrl) =>
            services.AddScoped<IAuthApiClient, AuthApiClient>(sp => new AuthApiClient(GetAuthorizedHttpClient(sp, serviceUrl)));

        private static AuthorizedHttpClientBase GetAuthorizedHttpClient(IServiceProvider sp, string serviceUrl) =>
            new AuthorizedHttpClientBase(
                sp.GetRequiredService<IHttpClientFactory>(),
                sp.GetRequiredService<IOptionsMonitor<AuthByPasswordOptions>>(),
                serviceUrl
            );
    }
}
