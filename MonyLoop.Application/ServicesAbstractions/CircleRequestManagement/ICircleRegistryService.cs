using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.CircleRequestManagement;
using MonyLoop.Domain.Constants;

namespace MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;

public interface ICircleRegistryService
{
    Task<Result<IReadOnlyList<CircleResponseDto>>> GetAllAsync(CircleStatus? status = null, CancellationToken cancellationToken = default);
    Task<Result<CircleResponseDto>> GetByIdAsync(Guid circleId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<CircleSlotResponseDto>>> GetSlotsAsync(Guid circleId, CancellationToken cancellationToken = default);
    Task<Result<CircleSlotResponseDto>> VacateSlotAsync(Guid actorUserId, Guid circleId, int slotNumber, CancellationToken cancellationToken = default);
}
