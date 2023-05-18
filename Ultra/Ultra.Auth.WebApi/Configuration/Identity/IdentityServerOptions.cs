namespace Ultra.Auth.WebApi.Configuration.Identity
{
    public class IdentityServerOptions
    {
        public string ApiSecret { get; set; }

        public string ApiName { get; set; }

        public string ClientSecret { get; set; }

        public string AuthorityUrl { get; set; }

        public int AccessTokenLifetime { get; set; }

        public int RefreshTokenLifetime { get; set; }
    }
}
