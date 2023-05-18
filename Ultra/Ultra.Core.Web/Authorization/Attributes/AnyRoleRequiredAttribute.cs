using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using IdentityModel;
using Ultra.Core.Services.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace Ultra.Core.Web.Authorization.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AnyRoleRequiredAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _requiredRoles;

        public AnyRoleRequiredAttribute(string role)
        {
            _requiredRoles = new[] { role };
        }

        public AnyRoleRequiredAttribute(params string[] roles)
        {
            _requiredRoles = roles;
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
            var roles = accessor.GetRoles();
            var isAuthorized = _requiredRoles.Any(requiredRole => roles.Contains(requiredRole));
            if (!isAuthorized)
            {
                context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
                return;
            }
        }
    }
}
