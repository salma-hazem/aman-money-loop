using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.Common;

namespace MonyLoop.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiBaseController : ControllerBase
    {
        protected ActionResult<T> FromResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return Ok(result.Value);

            var error = result.Errors.First();

            return error.Type switch
            {
                ErrorType.NotFound => NotFound(error),
                ErrorType.Validation => BadRequest(error),
                ErrorType.Unauthorized => Unauthorized(error),
                ErrorType.Forbidden => Forbid(),
                ErrorType.InvalidCredentials => BadRequest(error),
                ErrorType.Failure => StatusCode(StatusCodes.Status500InternalServerError, error),
                _ => StatusCode(StatusCodes.Status500InternalServerError, error)
            };
        }
    }
}