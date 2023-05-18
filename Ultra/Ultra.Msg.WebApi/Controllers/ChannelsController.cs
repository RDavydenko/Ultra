using Microsoft.AspNetCore.Mvc;
using Ultra.Core.Web.Authorization.Attributes;
using Ultra.Core.Web.Authorization;
using Ultra.Core.Web.Controllers;
using Ultra.Msg.WebApi.Models.Message;
using Ultra.Msg.WebApi.Services.Interfaces;
using Ultra.Msg.WebApi.Models.Channel;
using Ultra.Infrastructure.Models;

namespace Ultra.Msg.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChannelsController : UltraControllerBase
    {
        private readonly IChannelService _service;

        public ChannelsController(IChannelService service)
        {
            _service = service;
        }

        [HttpGet]
        [PermissionRequired(AuthEntity.Chat, AuthAction.View)]
        public Task<IActionResult> GetChannels([FromQuery] PageModel? pageModel = null)
            => GetResultAsync(_service.GetChannels(pageModel));

        [HttpGet("unreadMessagesCount")]
        [PermissionRequired(AuthEntity.Chat, AuthAction.View)]
        public Task<IActionResult> GetUnreadMessagesCount()
            => GetResultAsync(_service.GetUnreadMessagesCount());

        [HttpGet("{channelId:int}")]
        [PermissionRequired(AuthEntity.Chat, AuthAction.View)]
        public Task<IActionResult> GetChannel([FromRoute] int channelId)
            => GetResultAsync(_service.GetChannel(channelId));

        [HttpGet("{channelId:int}/messages")]
        [PermissionRequired(AuthEntity.Chat, AuthAction.View)]
        public Task<IActionResult> GetMessages([FromRoute] int channelId, [FromQuery] PageModel? model = null)
            => GetResultAsync(_service.GetMessages(channelId, model));

        [HttpPost]
        [PermissionRequired(AuthEntity.Chat, AuthAction.Send)]
        public Task<IActionResult> CreateChannel([FromBody] ChannelCreateModel model)
            => GetResultAsync(_service.CreateChannel(model));

        [HttpPost("{channelId:int}/send")]
        [PermissionRequired(AuthEntity.Chat, AuthAction.Send)]
        public Task<IActionResult> SendMessage([FromRoute] int channelId, [FromBody] SendMessageModel model)
            => GetResultAsync(_service.SendMessage(channelId, model));

        [HttpPost("{channelId:int}/read")]
        [PermissionRequired(AuthEntity.Chat, AuthAction.View)]
        public async Task<IActionResult> ReadMessagesFromChannel([FromRoute] int channelId)
        {
            await _service.ReadMessages(channelId);
            return OkResult();
        }

        [HttpPost("{channelId:int}/receive")]
        [PermissionRequired(AuthEntity.Chat, AuthAction.View)]
        public async Task<IActionResult> ReceiveMessagesFromChannel([FromRoute] int channelId)
        {
            await _service.ReceiveMessages(channelId);
            return OkResult();
        }
    }
}
