using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.CircleRequestManagement;

namespace MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;

public interface ISlotAssignmentService
{
    Task<Result<CircleSlotResponseDto>> AssignMemberLedgerAsync(Guid actorUserId, Guid circleId, int slotNumber, Guid memberLedgerId, CancellationToken cancellationToken = default);
}
