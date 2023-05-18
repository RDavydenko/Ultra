using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Ultra.Core.Web.Features;
using Ultra.Core.Web.Conventions;
using Ultra.Core.Services.Abstract;
using Ultra.Core.Services;
using Ultra.Core.Tools;
using Microsoft.AspNetCore.Authentication;
using Ultra.Infrastructure.Models.Auth.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System;
using Z.EntityFramework.Extensions;
using Ultra.Core.DAL;
using Ultra.Core.Extensions;
using Ultra.Core.Entities.Extensions;
using Ultra.Core.Services.CrudService;
using System.Text.Json.Serialization;
using Ultra.Core.Web.Services.Background;
using Mapster;
using Ultra.Core.Services.Providers.Abstract;
using Ultra.Core.Services.Providers;

namespace Ultra.Core.Web.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IMvcBuilder AddCrmControllers(this IMvcBuilder builder)
        {
            Executor.WebAssembly = Assembly.GetEntryAssembly();

            return builder
                .AddApplicationPart(Assembly.GetExecutingAssembly())
                .AddMvcOptions(
                    o =>
                    {
                        o.Conventions.Add(new CrudControllerRouteConvention());
                        o.Conventions.Add(new HideUnavailableCrudActionsConvention());
                    })
                .ConfigureApplicationPartManager(
                    m =>
                    {
                        m.FeatureProviders.Add(new CrudControllerFeatureProvider(Executor.WebAssembly));
                        m.FeatureProviders.Add(new CrmControllersFeatureProvider());
                    });
        }

        public static IServiceCollection AddUltraServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IUserAccessor, UserAccessor>();

            return services;
        }

        public static IServiceCollection AddCrmServices(this IServiceCollection services)
        {
            Executor.WebAssembly = Assembly.GetEntryAssembly();

            TypeAdapterConfig.GlobalSettings.Scan(typeof(Mapping.EntityTypeMapping).Assembly);
            services.AddHostedService<UpdateEntityTypesBackgroundService>();

            services.AddScoped(typeof(IPermissionService<>), typeof(PermissionService<>));
            services.AddCrudService();
            services.AddScoped<ICrudServiceHelper, CrudServiceHelper>();
            services.AddScoped<IEntityProvider, EntityProvider>();
            services.AddScoped<IEntityService, EntityService>();

            return services;
        }

        public static IServiceCollection AddUltraDbContext<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> builder)
            where TContext : UltraDbContextBase
        {
            // Using a constructor that requires optionsBuilder (EF Core) 
            EntityFrameworkManager.ContextFactory = context =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<TContext>();
                builder(optionsBuilder);
                return (TContext)Activator.CreateInstance(typeof(TContext), optionsBuilder.Options)!;
            };

            services.AddDbContextPool<TContext>(builder);
            services.AddScoped<UltraDbContextBase, TContext>();

            return services;
        }

        public static AuthenticationBuilder AddIdentityServerAuthenticationOptions(this IServiceCollection services, AuthServerOptions authServerOptions)
        {
            return services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.Authority = authServerOptions.AuthorityUrl;
                    options.ApiName = authServerOptions.ApiName;
                    options.ApiSecret = authServerOptions.ApiSecret;
                    options.SupportedTokens = SupportedTokens.Jwt;
                });
        }

        public static IServiceCollection AddCrudService(this IServiceCollection services)
        {
            var entities = Executor.WebAssembly.GetDbEntities();

            foreach (var entity in entities)
            {
                var crudInterfaceType = typeof(ICrudService<>).MakeGenericType(entity);
                var crudServiceType = typeof(CrudService<>).MakeGenericType(entity);

                services.AddScoped(crudInterfaceType, provider =>
                {
                    var crudService = ActivatorUtilities.CreateInstance(provider, crudServiceType);

                    if (entity.IsCrudEntity())
                    {
                        var crudCrudServiceType = typeof(CrudCrudService<>).MakeGenericType(entity);
                        crudService = ActivatorUtilities.CreateInstance(provider, crudCrudServiceType);
                    }

                    return crudService;
                });
            }

            return services;
        }
    }
}
