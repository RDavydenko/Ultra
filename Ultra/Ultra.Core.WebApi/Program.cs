using HotChocolate.Execution.Options;
using HotChocolate.Types.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ultra.Core.Web.Extensions;
using Ultra.Core.Web.GraphQL;
using Ultra.Core.Web.Swagger;
using Ultra.Core.Web.Swagger.Filters;
using Ultra.Core.WebApi.Configuration;
using Ultra.Core.WebApi.DAL;
using Ultra.Core.WebApi.DAL.Extensions;
using Ultra.Core.WebApi.Web.GraphQL;
using Ultra.Core.WebApi.Web.JsonConverters;
using Ultra.Infrastructure.Extensions;
using Ultra.Infrastructure.Models.Auth.Configuration;

namespace Ultra.Core.WebApi
{
    public class Program
    {
        private const string Policy = "CrmApiPolicy";
        private static IConfiguration _configuration;
        private static AuthServerOptions _authServerOptions;

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            _configuration = builder.Configuration;
            _authServerOptions = _configuration.GetSection("AuthServerOptions").Get<AuthServerOptions>();

            ConfigureServices(builder.Services);

            var app = builder.Build();
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            Configure(app, app.Services.CreateScope().ServiceProvider);

            app.Run();
        }

        // Configure application services
        private static void ConfigureServices(IServiceCollection services)
        {
            ConfigureOptions(services);
            ConfigureApiClients(services);
            ConfigureApplicationServices(services);

            services.AddControllers()
                .AddCrmControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.Converters.Add(new PointJsonConverter());
                });

            var connectionString = _configuration.GetConnectionString("DbContext");
            services
                .AddUltraServices()
                .AddCrmServices()
                .AddUltraDbContext<UltraDbContext>(options =>
                {
                    options.UseNpgsql(connectionString, x => x.UseNetTopologySuite());
                    options.LogTo(Console.WriteLine, LogLevel.Information);
                    options.UseUpperSnakeCaseNamingConvention();
                });

            services
                .AddAuthorization(x => x.AddPolicy(Policy, b => b.RequireScope(_authServerOptions.ApiScope)))
                .AddIdentityServerAuthenticationOptions(_authServerOptions);

            ConfigureCrmGraphQL(services);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                SwaggerConfigurator.ConfigureBearerAuthorization(c);
                c.OperationFilter<GenerateCrudControllerParametersOperationFilter>();
                c.SchemaFilter<HideUnavailableCrudPropertiesSchemaFilter>();
            });

            services.AddCors();
        }

        private static void ConfigureApiClients(IServiceCollection services)
        {
            var servicesConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<ServicesConfiguration>>().Value;
            services.AddAuthApiClient(servicesConfiguration.AuthService);
        }

        private static void ConfigureCrmGraphQL(IServiceCollection services)
        {
            services.AddGraphQLServer()
                .AddQueryType(QueryGenerator.Generate())
                .AddType<PointSortType>()
                .AddSpatialTypes()
                .AddFiltering().AddSpatialFiltering()
                .AddProjections().AddSpatialProjections()
                .AddSorting()
                .SetRequestOptions(_ => new RequestExecutorOptions { ExecutionTimeout = TimeSpan.FromMinutes(1) })
                .SetPagingOptions(new PagingOptions { IncludeTotalCount = true, DefaultPageSize = 100, MaxPageSize = 1000 })
                .InitializeOnStartup();
        }

        private static void ConfigureOptions(IServiceCollection services)
        {
            services.Configure<AuthByPasswordOptions>(_configuration.GetSection("AuthByPasswordOptions"));
            services.Configure<ServicesConfiguration>(_configuration.GetSection("ServicesConfiguration"));
        }

        private static void ConfigureApplicationServices(IServiceCollection services)
        {

        }

        // Configure the HTTP request pipeline.
        private static void Configure(WebApplication app, IServiceProvider provider)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors(c => c
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers()
                .RequireAuthorization(Policy);

            app.MapGraphQL()
                .RequireAuthorization(Policy);

            if (app.Environment.IsDevelopment())
            {
                provider.GetRequiredService<UltraDbContext>().SeedAsync(provider).ConfigureAwait(false);
            }
        }
    }
}