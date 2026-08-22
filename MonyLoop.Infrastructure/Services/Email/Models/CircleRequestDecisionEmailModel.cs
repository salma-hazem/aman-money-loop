namespace MonyLoop.Infrastructure.Services.Email.Models;

public sealed class CircleRequestDecisionEmailModel
{
    public string OrganizerName { get; init; } = string.Empty;
    public Guid RequestId { get; init; }
    public string CircleTitle { get; init; } = string.Empty;
    public string RequestStatus { get; init; } = string.Empty;
    public string? DecisionReason { get; init; }
    public DateTime ReviewedAt { get; init; }
}
