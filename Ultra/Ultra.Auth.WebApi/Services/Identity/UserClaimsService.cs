using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using System.Text.Json;
using Ultra.Auth.WebApi.DAL;
using Ultra.Auth.WebApi.DAL.Entities;
using Ultra.Core.DAL.Extensions;
using Ultra.Core.Models.Auth;
using Ultra.Core.Extensions;

namespace Ultra.Auth.WebApi.Services.Identity
{
    internal interface IUserClaimsService
    {
        IEnumerable<Claim> GetUserClaims(User user);
    }

    internal class UserClaimsService : IUserClaimsService
    {
        public UserClaimsService()
        {
        }

        // TODO: почему вызывается два раза при авторизации?
        public IEnumerable<Claim> GetUserClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(CustomClaimTypes.Id, user.Id.ToString()),
                new Claim(CustomClaimTypes.Name, user.Login),
                new Claim(CustomClaimTypes.UserName, user.UserName),
            };

            var activeRoles = user.Roles.Where(x => x.Role.IsActive()).ToList();
            foreach (var role in activeRoles)
            {
                claims.Add(new Claim(CustomClaimTypes.Role, role.Role.Code));
            }

            var activePermissions = activeRoles
                .SelectMany(x => x.Role.Permissions)
                .Where(x => x.Permission.IsActive())
                .Select(x => new { x.Permission.Entity, x.Permission.Action })
                .Distinct()
                .ToList();

            foreach (var permission in activePermissions)
            {
                claims.Add(new Claim(CustomClaimTypes.Permission, $"{permission.Entity},{permission.Action}"));
            }

            return claims;
        }
    }
}
