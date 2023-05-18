using IdentityModel;

namespace Ultra.Core.Models.Auth
{
    public static class CustomClaimTypes
    {
        public const string Id = JwtClaimTypes.Id;
        public const string Name = JwtClaimTypes.Name;
        public const string Role = JwtClaimTypes.Role;
        public const string UserName = JwtClaimTypes.GivenName;
        public const string Permission = "permission";
    }
}
