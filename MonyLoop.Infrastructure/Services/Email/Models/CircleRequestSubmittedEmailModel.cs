namespace MonyLoop.Infrastructure.Services.Email.Models;

public sealed class CircleRequestSubmittedEmailModel
{
    public string RecipientName { get; init; } = string.Empty;
    public Guid RequestId { get; init; }
    public string CircleTitle { get; init; } = string.Empty;
    public string CircleType { get; init; } = string.Empty;
    public DateTime SubmittedAt { get; init; }
}
