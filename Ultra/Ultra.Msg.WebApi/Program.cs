using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ultra.Core.Web.Extensions;
using Ultra.Core.Web.Swagger;
using Ultra.Infrastructure.Extensions;
using Ultra.Infrastructure.Models.Auth.Configuration;
using Ultra.Msg.WebApi.Configuration;
using Ultra.Msg.WebApi.DAL;
using Ultra.Msg.WebApi.Extensions;
using Ultra.Msg.WebApi.Hubs;
using Ultra.Msg.WebApi.Hubs.Interfaces;
using Ultra.Msg.WebApi.Hubs.Repositories;
using Ultra.Msg.WebApi.Services;
using Ultra.Msg.WebApi.Services.Interfaces;

namespace Ultra.Msg.WebApi
{
    public class Program
    {
        private const string Policy = "MsgApiPolicy";
        private static IConfiguration _configuration;
        private static AuthServerOptions _authServerOptions;

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            _configuration = builder.Configuration;
            _authServerOptions = _configuration.GetSection("AuthServerOptions").Get<AuthServerOptions>();

            ConfigureServices(builder.Services);

            var app = builder.Build();

            Configure(app, app.Services.CreateScope().ServiceProvider);

            app.Run();
        }

        // Configure application services
        private static void ConfigureServices(IServiceCollection services)
        {
            ConfigureOptions(services);
            ConfigureDbContext(services);
            ConfigureApiClients(services);
            ConfigureApplicationServices(services);
            ConfigureHubs(services);

            services.AddControllers();

            services
                .AddAuthorization(x => x.AddPolicy(Policy, b => b.RequireScope(_authServerOptions.ApiScope)))
                .AddIdentityServerAuthenticationOptions(_authServerOptions);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                SwaggerConfigurator.ConfigureBearerAuthorization(c);
            });

            services.AddCors();

            services.AddSignalR();
        }

        private static void ConfigureHubs(IServiceCollection services)
        {
            services.AddSingleton<UserHubRepository>();
            //services.AddSingleton<IChatHub, ChatHub>();
        }

        private static void ConfigureApiClients(IServiceCollection services)
        {
            var servicesConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<ServicesConfiguration>>().Value;
            services.AddAuthApiClient(servicesConfiguration.AuthService);
        }

        private static void ConfigureOptions(IServiceCollection services)
        {
            services.Configure<AuthByPasswordOptions>(_configuration.GetSection("AuthByPasswordOptions"));
            services.Configure<ServicesConfiguration>(_configuration.GetSection("ServicesConfiguration"));
        }

        private static void ConfigureApplicationServices(IServiceCollection services)
        {
            services.AddMapster();
            services.AddUltraServices();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IChannelService, ChannelService>();
        }

        private static void ConfigureDbContext(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("DbContext");
            services.AddDbContext<MsgDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            }, ServiceLifetime.Scoped);
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

            app.UseRouting();
            app.UseCors(c => c
                .SetIsOriginAllowed((host) => true) //.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization(Policy);
                endpoints.MapHub<ChatHub>("/hubs/chat").RequireAuthorization(Policy);
            });
        }
    }
}