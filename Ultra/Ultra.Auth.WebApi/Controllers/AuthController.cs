using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ultra.Core.Web.Controllers;

namespace Ultra.Auth.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : UltraControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult CheckAuth() => Ok();
    }
}
