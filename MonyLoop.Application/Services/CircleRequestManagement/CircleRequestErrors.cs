using MonyLoop.Application.Common;

namespace MonyLoop.Application.Services.CircleRequestManagement;

internal static class CircleRequestErrors
{
    public static Error RequestNotFound => Error.NotFound("CircleRequest.NotFound", "Circle request not found.");
    public static Error CircleNotFound => Error.NotFound("Circle.NotFound", "Circle not found.");
    public static Error ListingNotFound => Error.NotFound("MarketplaceListing.NotFound", "Active marketplace listing not found.");
    public static Error SlotNotFound => Error.NotFound("CircleSlot.NotFound", "Circle slot not found.");
    public static Error Forbidden => Error.Forbidden("CircleRequest.Forbidden", "You do not own this circle request.");

    public static Error InvalidTransition(string currentStatus, string action) =>
        Error.Validation("CircleRequest.InvalidTransition", $"Cannot {action} a circle request in status '{currentStatus}'.");

    public static Error InvalidReplacement(string description) =>
        Error.Validation("CircleRequest.InvalidReplacement", description);
}
