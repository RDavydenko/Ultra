using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Ultra.Core.Services.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace Ultra.Core.Web.Authorization.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PermissionRequiredAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _entity;
        private readonly string _action;

        public PermissionRequiredAttribute(string entity, string action)
        {
            _entity = entity;
            _action = action;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                // it isn't needed to set unauthorized result 
                // as the base class already requires the user to be authenticated
                // this also makes redirect to a login page work properly
                // context.Result = new UnauthorizedResult();
                return;
            }

            var accessor = context.HttpContext.RequestServices.GetRequiredService<IUserAccessor>();
            var permissions = accessor.GetPermissions();
            var isAuthorized = permissions.Any(permission => permission.Entity == _entity && permission.Action == _action);
            if (!isAuthorized)
            {
                context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
                return;
            }
        }
    }
}
