using Microsoft.AspNetCore.Mvc;

namespace MonyLoop.API.Factories
{
    public class ApiResponseFactory
    {
        public static IActionResult GenerateApiValidationResponse(ActionContext actionContext)
        {
            var errors = actionContext.ModelState
                .Where(e => e.Value!.Errors.Count > 0)
                .ToDictionary(x => x.Key, x => x.Value!.Errors.Select(e => e.ErrorMessage))
                .ToArray();

            var problem = new ProblemDetails
            {
                Title = "Validation Errors",
                Detail = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Extensions = { { "errors", errors } }
            };

            return new BadRequestObjectResult(problem);
        }
    }
}
