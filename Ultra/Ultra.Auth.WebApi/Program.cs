using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Ultra.Auth.WebApi.Configuration.Identity;
using Ultra.Auth.WebApi.DAL;
using Ultra.Auth.WebApi.DAL.Extensions;
using Ultra.Auth.WebApi.Services;
using Ultra.Auth.WebApi.Services.Identity;
using Ultra.Auth.WebApi.Services.Interfaces;
using Ultra.Auth.WebApi.Web.Extensions;
using Ultra.Core.DAL;
using Ultra.Core.Web.Extensions;
using Ultra.Core.Web.Swagger;
using Ultra.Infrastructure.Models.Auth.Configuration;

namespace Ultra.Auth.WebApi
{
    public class Program
    {
        private const string Policy = "AuthApiPolicy";

        private static IConfiguration _configuration;
        private static IWebHostEnvironment _environment;
        private static AuthServerOptions _authServerOptions;

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            _configuration = builder.Configuration;
            _environment = builder.Environment;
            _authServerOptions = _configuration.GetSection("AuthServerOptions").Get<AuthServerOptions>();

            // Add services to the container.
            ConfigureServices(builder.Services);

            var app = builder.Build();

            Configure(app, app.Services.CreateScope().ServiceProvider);

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            ConfigureOptions(services);
            ConfigureDbContext(services);
            ConfigureApplicationServices(services);

            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(SwaggerConfigurator.ConfigureBearerAuthorization);

            services.AddCors();
            services.AddMemoryCache();

            ConfigureIdentityServer(services);

            services
                .AddAuthorization(x => x.AddPolicy(Policy, b => b.RequireScope(_authServerOptions.ApiScope)))
                .AddIdentityServerAuthenticationOptions(_authServerOptions);
        }

        private static void ConfigureDbContext(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("DbContext");
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseNpgsql(connectionString);                
            }, ServiceLifetime.Scoped);
        }

        private static void ConfigureApplicationServices(IServiceCollection services)
        {
            services.AddUltraServices();
            services.AddMapster();
            services.AddSingleton<IPasswordHashService, PasswordHashService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserClaimsService, UserClaimsService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEntityPermissionService, EntityPermissionService>();
        }

        private static void ConfigureOptions(IServiceCollection services)
        {
            services.AddOptions<AuthServerOptions>().Bind(_configuration.GetSection("AuthServerOptions"));
        }

        private static void ConfigureIdentityServer(IServiceCollection services)
        {
            var authOptions = _configuration.GetSection("IdentityServer").Get<IdentityServerOptions>();
            var config = new IdentityServerConfig(authOptions);
            var identityBuilder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.IssuerUri = _authServerOptions.AuthorityUrl;
            })
            .AddInMemoryApiResources(config.ApiResources)
            .AddInMemoryApiScopes(config.Scopes)
            .AddInMemoryIdentityResources(config.IdentityResources)
            .AddInMemoryClients(config.Clients)
            .AddProfileService<ProfileService>()
            .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();

            if (_environment.IsDevelopment())
            {
                identityBuilder.AddDeveloperSigningCredential(false);
            }
            else
            {
                var key = _configuration.GetValue<string>("JwtSignKey");
                var keyBytes = Convert.FromBase64String(key);
                var ecdsa = ECDsa.Create();
                ecdsa.ImportECPrivateKey(keyBytes, out _);
                identityBuilder.AddSigningCredential(new ECDsaSecurityKey(ecdsa), IdentityServerConstants.ECDsaSigningAlgorithm.ES256);
            }
        }

        private static void Configure(WebApplication app, IServiceProvider provider)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(c => c
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                    .RequireAuthorization(Policy);
            });

            if (app.Environment.IsDevelopment())
            {
                provider.GetRequiredService<AuthDbContext>().SeedAsync(provider).ConfigureAwait(false);
            }
        }
    }
}