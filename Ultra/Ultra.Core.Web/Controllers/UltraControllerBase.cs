using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Ultra.Infrastructure.Models;

namespace Ultra.Core.Web.Controllers
{
    public class UltraControllerBase : ControllerBase
    {
        protected IActionResult OkResult()
        {
            return GetResult(new Result() {  StatusCode = HttpStatusCode.OK });
        }

        protected async Task<IActionResult> GetResultAsync<T>(Task<Result<T>> task)
        {
            return GetResult(await task);
        }

        protected IActionResult GetResult<T>(Result<T> result)
        {
            return Ok(result);
        }

        protected async Task<IActionResult> GetResultAsync(Task<Result> task)
        {
            return GetResult(await task);
        }

        protected IActionResult GetResult(Result result)
        {
            return Ok(result);
        }
    }
}
