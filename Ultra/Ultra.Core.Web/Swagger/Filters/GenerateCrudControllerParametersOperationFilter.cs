using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultra.Core.Entities.Abstract;
using Ultra.Core.Web.Controllers;
using Ultra.Core.Web.Controllers.Abstract;

namespace Ultra.Core.Web.Swagger.Filters
{
    public class GenerateCrudControllerParametersOperationFilter : IOperationFilter
    {
         // TODO:

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

            var descriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;

            if (descriptor != null && descriptor.ControllerTypeInfo.IsAssignableTo(typeof(ICrudController)))
            {
                if (descriptor.ActionName == nameof(CrudController<EntityBase>.Create))
                {
                    //operation.RequestBody = new OpenApiRequestBody()
                    //{
                    //    Content = new Dictionary<string, OpenApiMediaType>()
                    //    {
                    //        { "hello", new OpenApiMediaType()
                    //        {
                    //            Schema = new OpenApiSchema()
                    //            {
                    //                Type = typeof(bool).ToString()
                    //            }
                    //        } }
                    //    }
                    //};


                    //operation.Parameters.Add(new OpenApiParameter()
                    //{
                    //    Name = "sign",
                    //    In = ParameterLocation.Query,
                    //    Description = "The signature",
                    //    Required = true
                    //});
                }
            }
        }
    }
}
