using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;
using Ultra.Core.Extensions;
using Ultra.Core.Models.Auth;
using Ultra.Extensions;

namespace Ultra.Auth.WebApi.Services.Identity
{
    internal class ProfileService : IProfileService
    {
        private readonly ILogger<ProfileService> _logger;
        private readonly IAuthenticationService _authService;
        private readonly IUserClaimsService _claimsService;

        public ProfileService(
            ILogger<ProfileService> logger,
            IAuthenticationService authService,
            IUserClaimsService claimsService)
        {
            _logger = logger;
            _authService = authService;
            _claimsService = claimsService;
        }

        //Get user profile date in terms of claims when calling /connect/userinfo
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            try
            {
                var login = context.Subject?.Identity?.Name;
                if (login.IsNotNullOrEmpty())
                {
                    //get user from db (in my case this is by name)
                    var user = (await _authService.GetUser(login!)).GetObjectOrThrow();
                    var claims = _claimsService.GetUserClaims(user);
                    context.IssuedClaims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                }
                else
                {
                    //get subject from context (this was set ResourceOwnerPasswordValidator.ValidateAsync),
                    //where and subject was set to my user id.
                    var userIdClaim = context.Subject?.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id);

                    if (int.TryParse(userIdClaim?.Value, out var userId))
                    {
                        //get user from db (find user by user id)
                        var user = (await _authService.GetUser(userId)).GetObjectOrThrow();
                        var claims = _claimsService.GetUserClaims(user);
                        context.IssuedClaims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        //check if user account is active.
        public async Task IsActiveAsync(IsActiveContext context)
        {
            try
            {
                var userIdClaim = context.Subject?.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Id);

                if (int.TryParse(userIdClaim?.Value, out var userId))
                {
                    context.IsActive = await _authService.IsUserExists(userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }
    }
}
