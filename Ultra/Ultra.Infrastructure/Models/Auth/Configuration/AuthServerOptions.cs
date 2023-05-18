namespace Ultra.Infrastructure.Models.Auth.Configuration
{
    public class AuthServerOptions
    {
        public string AuthorityUrl { get; set; }
        public string ApiName { get; set; }
        public string ApiScope { get; set; }
        public string ApiSecret { get; set; }
    }
}
