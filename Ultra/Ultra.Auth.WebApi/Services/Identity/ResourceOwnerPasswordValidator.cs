using IdentityServer4.Models;
using IdentityServer4.Validation;
using Ultra.Core.Extensions;
using Ultra.Extensions;

namespace Ultra.Auth.WebApi.Services.Identity
{
    internal class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ILogger<ResourceOwnerPasswordValidator> _logger;
        private readonly IAuthenticationService _authService;
        private readonly IUserClaimsService _claimsService;
        private readonly IPasswordHashService _hashService;

        public ResourceOwnerPasswordValidator(
            ILogger<ResourceOwnerPasswordValidator> logger,
            IAuthenticationService authService,
            IUserClaimsService claimsService,
            IPasswordHashService hashService)
        {
            _logger = logger;
            _authService = authService;
            _claimsService = claimsService;
            _hashService = hashService;
        }

        //this is used to validate your user account with provided grant at /connect/token
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                //get your user model from db
                var user = (await _authService.GetUser(context.UserName)).GetObjectOrThrow();
                
                //check if password match - remember to hash password if stored as hash in db
                if (user.PasswordHash == _hashService.GetHash(context.Password, user.Salt.FromBase64()))
                {
                    //set the result
                    context.Result = new GrantValidationResult(
                        subject: user.Id.ToString(),
                        authenticationMethod: "custom",
                        claims: _claimsService.GetUserClaims(user));
                }
                else
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Incorrect password");
                }
            }
            catch (Exception ex)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
                _logger.LogError(ex);
            }
        }
    }
}
