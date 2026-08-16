using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.CircleRequestManagement;

namespace MonyLoop.Domain.Interfaces.CircleRequestManagement;

public interface ICircleRequestRepository
{
    Task<CircleRequest?> GetByIdAsync(
        Guid requestId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CircleRequest>> GetByOrganizerIdAsync(
        Guid organizerId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CircleRequest>> GetByStatusAsync(
        CircleRequestStatus status,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CircleRequest>> GetReplacementRequestsAsync(
        Guid existingCircleId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid requestId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        CircleRequest request,
        CancellationToken cancellationToken = default);
}
