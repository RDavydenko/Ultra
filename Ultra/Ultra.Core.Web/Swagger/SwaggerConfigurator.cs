using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultra.Core.Web.Swagger
{
    public static class SwaggerConfigurator
    {
        public static void ConfigureBearerAuthorization(SwaggerGenOptions c)
        {
            c.AddSecurityDefinition("Bearer Token", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                BearerFormat = "JWT",
                Scheme = "bearer",
                Description = "Specify the authorization token.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http
            });
            OpenApiSecurityRequirement securityRequirement = new();
            securityRequirement.Add(new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Id = "Bearer Token",
                    Type = new ReferenceType?(ReferenceType.SecurityScheme)
                }
            }, (IList<string>)Array.Empty<string>());
            c.AddSecurityRequirement(securityRequirement);
        }
    }
}
