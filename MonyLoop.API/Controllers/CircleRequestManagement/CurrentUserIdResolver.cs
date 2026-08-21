using System.Security.Claims;

namespace MonyLoop.API.Controllers.CircleRequestManagement;

internal static class CurrentUserIdResolver
{
    public static bool TryGet(ClaimsPrincipal user, out Guid userId)
    {
        var standardClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(standardClaim, out userId))
        {
            return true;
        }

        var legacyClaim = user.FindFirstValue("uid");
        return Guid.TryParse(legacyClaim, out userId);
    }
}
