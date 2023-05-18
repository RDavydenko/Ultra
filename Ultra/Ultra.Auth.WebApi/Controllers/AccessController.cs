using Microsoft.AspNetCore.Mvc;
using Ultra.Auth.WebApi.Services.Interfaces;
using Ultra.Core.Web.Authorization;
using Ultra.Core.Web.Authorization.Attributes;
using Ultra.Core.Web.Controllers;
using Ultra.Infrastructure.Models.Enums;

namespace Ultra.Auth.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccessController : UltraControllerBase
    {
        private readonly IEntityPermissionService _entityPermissionService;

        public AccessController(IEntityPermissionService entityPermissionService)
        {
            _entityPermissionService = entityPermissionService;
        }

        [HttpGet("{sysName}/view")]
        [PermissionRequired(AuthEntity.User, AuthAction.View)]
        public Task<IActionResult> GetViewPermissionModel([FromRoute] string sysName, [FromQuery] int userId)
            => GetResultAsync(_entityPermissionService.GetViewPermissionModel(sysName, userId));

        [HttpGet("{sysName}/entities/{method}")]
        [PermissionRequired(AuthEntity.User, AuthAction.View)]
        public Task<IActionResult> CanEntitiesAccessByMethod(
           [FromRoute] string sysName,
           [FromRoute] EntityMethod method,
           [FromQuery] int userId)
           => GetResultAsync(_entityPermissionService.CanEntityAccessByMethod(sysName, method, userId));

        [HttpGet("{sysName}/entity/{id:int}/{method}")]
        [PermissionRequired(AuthEntity.User, AuthAction.View)]
        public Task<IActionResult> CanEntityAccessByMethod(
            [FromRoute] string sysName, 
            [FromRoute] int id, 
            [FromRoute] EntityMethod method,
            [FromQuery] int userId)
            => GetResultAsync(_entityPermissionService.CanEntityAccessByMethod(sysName, id, method, userId));
    }
}
