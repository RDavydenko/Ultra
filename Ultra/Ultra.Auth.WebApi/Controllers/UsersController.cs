using Microsoft.AspNetCore.Mvc;
using Ultra.Auth.WebApi.Services.Interfaces;
using Ultra.Core.Web.Authorization;
using Ultra.Core.Web.Authorization.Attributes;
using Ultra.Core.Web.Controllers;

namespace Ultra.Auth.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : UltraControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public Task<IActionResult> GetCurrentUser()
            => GetResultAsync(_userService.GetCurrentUser());

        [HttpGet("{userId:int}")]
        [PermissionRequired(AuthEntity.User, AuthAction.View)]
        public Task<IActionResult> GetUser([FromRoute] int userId)
            => GetResultAsync(_userService.GetUser(userId));

        [HttpGet("{userId:int}/exists")]
        [PermissionRequired(AuthEntity.User, AuthAction.View)]
        public Task<IActionResult> ExistsUser([FromRoute] int userId)
            => GetResultAsync(_userService.ExistsUser(userId));

        [HttpGet("{userId:int}/userName")]
        [ResponseCache(Duration = 60)]
        [PermissionRequired(AuthEntity.User, AuthAction.View)]
        public Task<IActionResult> GetUserName([FromRoute] int userId)
            => GetResultAsync(_userService.GetUserName(userId));

        [HttpGet("search")]
        [PermissionRequired(AuthEntity.User, AuthAction.View)]
        public Task<IActionResult> SearchUsers([FromQuery] string? q = null, [FromQuery] int? id = null)
            => GetResultAsync(_userService.SearchUsers(q, id));

        [HttpGet("byLogin")]
        [PermissionRequired(AuthEntity.User, AuthAction.View)]
        public Task<IActionResult> GetUser([FromQuery] string login)
            => GetResultAsync(_userService.GetUser(login));
    }
}
