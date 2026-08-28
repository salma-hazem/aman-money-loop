using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.CircleRequestManagement;

namespace MonyLoop.Domain.Interfaces.CircleRequestManagement;

public interface ICircleRepository
{
    Task<IReadOnlyList<Circle>> GetAllAsync(
        CircleStatus? status = null,
        CancellationToken cancellationToken = default);

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
