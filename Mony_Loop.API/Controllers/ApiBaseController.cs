using Microsoft.AspNetCore.Mvc;
using Mony_Loop.Application.Common;

namespace Mony_Loop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiBaseController : ControllerBase
    {
        protected ActionResult<T> FromResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return Ok(result.Value);

            return result.Error!.Type switch
            {
                ErrorType.NotFound => NotFound(new { result.Error.Code, result.Error.Message }),
                ErrorType.Validation => BadRequest(new { result.Error.Code, result.Error.Message }),
                ErrorType.Conflict => Conflict(new { result.Error.Code, result.Error.Message }),
                ErrorType.Unauthorized => Unauthorized(new { result.Error.Code, result.Error.Message }),
                _ => BadRequest(new { result.Error.Code, result.Error.Message })
            };
        }
    }
}