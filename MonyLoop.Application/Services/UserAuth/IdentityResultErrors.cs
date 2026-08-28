using Microsoft.AspNetCore.Identity;
using MonyLoop.Application.Common;

namespace MonyLoop.Application.Services.UserAuth;

internal static class IdentityResultErrors
{
    public static List<Error> ToValidationErrors(this IdentityResult result) =>
        result.Errors
            .Select(error => Error.Validation(error.Code, error.Description))
            .ToList();
}
