using Mony_Loop.Domain.Constants;
using Mony_Loop.Domain.Entities.CircleRequestManagement;

namespace Mony_Loop.Domain.Interfaces.CircleRequestManagement;

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
