using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultra.Core.Services.Abstract;
using Ultra.Core.Web.Authorization;
using Ultra.Core.Web.Authorization.Attributes;
using Ultra.Infrastructure.Models;

namespace Ultra.Core.Web.Controllers
{
    [Route("[controller]")]
    internal class EntitiesController : UltraControllerBase
    {
        private readonly IEntityService _service;

        public EntitiesController(IEntityService service)
        {
            _service = service;
        }

        [HttpGet]
        [PermissionRequired(AuthEntity.Data, AuthAction.View)]
        public Task<IActionResult> GetEntities([FromQuery] PageModel? pageModel = null)
            => GetResultAsync(_service.GetEntityTypesPage(pageModel));

        [HttpGet("geo")]
        [PermissionRequired(AuthEntity.Map, AuthAction.View)]
        public Task<IActionResult> GetGeoEntities()
            => GetResultAsync(_service.GetGeoEntityTypes());

        [HttpGet("{sysName}/configuration")]
        [PermissionRequired(AuthEntity.Data, AuthAction.View)]
        public Task<IActionResult> GetConfiguration([FromRoute] string sysName)
            => GetResultAsync(_service.GetConfiguration(sysName));

        [HttpPost("{sysName}/favorite")]
        [PermissionRequired(AuthEntity.Data, AuthAction.View)]
        public Task<IActionResult> ToggleFavorite([FromRoute] string sysName)
            => GetResultAsync(_service.ToggleFavorite(sysName));

        [HttpGet("{sysName}/favorite/{id:int}")]
        [PermissionRequired(AuthEntity.Data, AuthAction.View)]
        public Task<IActionResult> GetEntityFavorite([FromRoute] string sysName, [FromRoute] int id)
            => GetResultAsync(_service.GetEntityFavorite(sysName, id));

        [HttpPost("{sysName}/favorite/{id:int}")]
        [PermissionRequired(AuthEntity.Data, AuthAction.View)]
        public Task<IActionResult> ToggleFavorite([FromRoute] string sysName, [FromRoute] int id)
            => GetResultAsync(_service.ToggleFavorite(sysName, id));
    }
}
