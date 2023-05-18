using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Ultra.Core.Entities.Extensions;
using Ultra.Core.Entities.Interfaces;
using Ultra.Core.Models;
using Ultra.Core.Services.Abstract;
using Ultra.Core.Web.Authorization.Attributes;
using Ultra.Core.Web.Authorization;
using Ultra.Core.Web.Controllers.Abstract;
using Ultra.Infrastructure.Models;
using System;

namespace Ultra.Core.Web.Controllers
{
    internal class CrudController<T> : UltraControllerBase, ICrudController
        where T : class, IEntity
    {
        private readonly ICrudService<T> _crudService;
        private readonly ILogger<CrudController<T>> _logger;

        public CrudController(
            ICrudService<T> crudService,
            ILogger<CrudController<T>> logger)
        {
            _crudService = crudService;
            _logger = logger;
        }

        [HttpGet]
        [PermissionRequired(AuthEntity.Data, AuthAction.View)]
        public async Task<IActionResult> GetAll()
        {
            var entities = await (await _crudService.GetQueryable()).ToListAsync();

            entities.RemoveHidden();

            return Ok(Result.Success(entities));
        }

        [HttpGet("{id}")]
        [PermissionRequired(AuthEntity.Data, AuthAction.View)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var res = await _crudService.GetAsync(id);
            if (!res) 
            { 
                return GetResult(res); 
            }

            res.Object.RemoveHidden();
            return GetResult(res);
        }

        [HttpPost]
        [PermissionRequired(AuthEntity.Data, AuthAction.CreateRecord)]
        public async Task<IActionResult> Create([FromBody] EntityModelWithRelatedLinks<T> model)
        {
            model.Entity = model.Entity?.ToCreated();
            var res = await _crudService.CreateAsync(model.Entity, model.LinksToAddOrUpdate, model.LinksToDelete);
            if (!res)
            {
                return GetResult(res);
            }

            res.Object.RemoveHidden();
            return GetResult(res);
        }

        [HttpPut("{id}")]
        [PermissionRequired(AuthEntity.Data, AuthAction.UpdateRecord)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] EntityModelWithRelatedLinks<T> model)
        {
            model.Entity = model.Entity?.ToUpdated();
            var res = await _crudService.UpdateAsync(id, model.Entity, model.LinksToAddOrUpdate, model.LinksToDelete);
            if (!res)
            {
                return GetResult(res);
            }

            res.Object.RemoveHidden();
            return GetResult(res);
        }

        [HttpPatch("{id}")]
        [PermissionRequired(AuthEntity.Data, AuthAction.UpdateRecord)]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] PatchModel<T> model)
        {
            model = model?.ToPatched();
            var res = await _crudService.PatchAsync(id, model);
            if (!res)
            {
                return GetResult(res);
            }

            res.Object.RemoveHidden();
            return GetResult(res);
        }

        [HttpDelete("{id}")]
        [PermissionRequired(AuthEntity.Data, AuthAction.DeleteRecord)]
        public Task<IActionResult> Delete([FromRoute] int id)
            => GetResultAsync(_crudService.DeleteAsync(id));
    }    
}
