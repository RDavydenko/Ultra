namespace Ultra.Infrastructure.Models.Auth.Configuration
{
    public class AuthByPasswordOptions
    {
        public string GrantType => "password";
        public string ClientId => "ultra_ui_client";
        public string IdentityUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
