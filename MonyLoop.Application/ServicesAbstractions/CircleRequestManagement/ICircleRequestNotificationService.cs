using MonyLoop.Domain.Entities.CircleRequestManagement;

namespace MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;

public interface ICircleRequestNotificationService
{
    Task NotifySubmittedAsync(CircleRequest request, CancellationToken cancellationToken = default);
    Task NotifyDecisionAsync(CircleRequest request, CancellationToken cancellationToken = default);
}
