using IdentityModel;
using IdentityServer4.Models;
using Ultra.Core.Models.Auth;
using Ultra.Auth.WebApi.Models;
using static IdentityServer4.IdentityServerConstants;

namespace Ultra.Auth.WebApi.Configuration.Identity
{
    /// <summary>
    /// Конфигурация IdentityServer
    /// </summary>
    public class IdentityServerConfig
    {
        private readonly IdentityServerOptions _options;
        private readonly IEnumerable<string> _claims = new[]
        {
            CustomClaimTypes.Id,
            CustomClaimTypes.Name,
            CustomClaimTypes.Role,
            CustomClaimTypes.UserName,
            CustomClaimTypes.Permission
        };

        public IdentityServerConfig(IdentityServerOptions options) =>
            _options = options;

        public IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
            };

        public IEnumerable<ApiScope> Scopes =>
            new[]
            {
                new ApiScope(ApiScopes.CrmApiScope, "CRM API Scope"),
                new ApiScope(ApiScopes.AuthApiScope, "Auth API Scope"),
                new ApiScope(ApiScopes.MsgApiScope, "Msg API Scope")
            };

        public IEnumerable<ApiResource> ApiResources =>
            new[]
            {
                new ApiResource(_options.ApiName, "Microservices APIs", _claims)
                {
                    ApiSecrets = { new Secret(_options.ApiSecret.Sha256()) },
                    Scopes =
                    {
                        ApiScopes.CrmApiScope,
                        ApiScopes.AuthApiScope,
                    }
                }
            };

        public IEnumerable<Client> Clients =>
            new[]
            {
                new Client
                {
                    ClientId = "m2m_client",
                    ClientName = "M2M",
                    ClientSecrets = { new Secret(_options.ClientSecret.Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes =
                    {
                        ApiScopes.CrmApiScope,
                        ApiScopes.AuthApiScope,
                        ApiScopes.MsgApiScope
                    },
                    IdentityTokenLifetime = _options.AccessTokenLifetime,
                    AccessTokenLifetime = _options.AccessTokenLifetime
                },
                new Client
                {
                    ClientId = "techuser_client",
                    ClientName = "Technical user",
                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes =
                    {
                        ApiScopes.CrmApiScope,
                        ApiScopes.AuthApiScope,
                        ApiScopes.MsgApiScope,
                    },
                    IdentityTokenLifetime = _options.AccessTokenLifetime,
                    AccessTokenLifetime = _options.AccessTokenLifetime
                },
                new Client
                {
                    ClientId = "ultra_ui_client",
                    ClientName = "Microservices applications interactive client",
                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes =
                    {
                        StandardScopes.OpenId,
                        StandardScopes.OfflineAccess,   // Scope to allow refresh_token generation

                        ApiScopes.CrmApiScope,
                        ApiScopes.AuthApiScope,
                        ApiScopes.MsgApiScope,
                    },
                    IdentityTokenLifetime = _options.AccessTokenLifetime,
                    AccessTokenLifetime = _options.AccessTokenLifetime,
                    AllowOfflineAccess = true,
                    AbsoluteRefreshTokenLifetime = _options.RefreshTokenLifetime
                }
            };
    }
}
