using IdentityModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using Ultra.Core.Models.Auth;
using Ultra.Core.Services.Abstract;

namespace Ultra.Core.Services
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _accessor;
        private ClaimsPrincipal? User => _accessor.HttpContext?.User;

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;
        public int UserId => int.Parse(User.Claims.First(x => x.Type == CustomClaimTypes.Id).Value);

        public UserAccessor(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string[] GetRoles()
        {
            if (IsAuthenticated)
            {
                return User.Claims.Where(x => x.Type == CustomClaimTypes.Role).Select(x => x.Value).ToArray();
            }

            return Array.Empty<string>();
        }

        public (string Entity, string Action)[] GetPermissions()
        {
            if (IsAuthenticated)
            {
                return User.Claims
                    .Where(x => x.Type == CustomClaimTypes.Permission)
                    .Select(x => 
                    {
                        var s = x.Value.Split(',');
                        return (s[0], s[1]);
                    })
                    .ToArray();
            }

            return Array.Empty<(string Entity, string Action)>();
        }
    }
}
