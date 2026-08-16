using Mony_Loop.Domain.Entities.CircleRequestManagement;

namespace Mony_Loop.Domain.Interfaces.CircleRequestManagement;

public interface ICircleRepository
{
    Task<Circle?> GetByIdAsync(
        Guid circleId,
        CancellationToken cancellationToken = default);

    Task<Circle?> GetDetailsByIdAsync(
        Guid circleId,
        CancellationToken cancellationToken = default);

    Task<Circle?> GetByRequestIdAsync(
        Guid requestId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsForRequestAsync(
        Guid requestId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Circle circle,
        CancellationToken cancellationToken = default);
}
