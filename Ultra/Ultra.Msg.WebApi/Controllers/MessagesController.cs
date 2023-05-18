using Microsoft.AspNetCore.Mvc;
using Ultra.Core.Web.Authorization.Attributes;
using Ultra.Core.Web.Authorization;
using Ultra.Core.Web.Controllers;
using Ultra.Msg.WebApi.Services.Interfaces;

namespace Ultra.Msg.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : UltraControllerBase
    {
        private readonly IMessageService _service;

        public MessagesController(IMessageService service)
        {
            _service = service;
        }

        [HttpPost("{messageGuid}/read")]
        [PermissionRequired(AuthEntity.Chat, AuthAction.View)]
        public async Task<IActionResult> ReadMessage([FromRoute] Guid messageGuid)
        {
            await _service.ReadMessage(messageGuid);
            return OkResult();
        }

        [HttpPost("{messageGuid}/receive")]
        [PermissionRequired(AuthEntity.Chat, AuthAction.View)]
        public async Task<IActionResult> ReceiveMessage([FromRoute] Guid messageGuid)
        {
            await _service.ReceiveMessage(messageGuid);
            return OkResult();
        }
    }
}
