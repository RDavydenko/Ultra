using Microsoft.AspNetCore.Mvc;
using Ultra.Core.Web.Controllers;

namespace Ultra.Core.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : UltraControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(
            ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok("Hello world");
        }
    }
}