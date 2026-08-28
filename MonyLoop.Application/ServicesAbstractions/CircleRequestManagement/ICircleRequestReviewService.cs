using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.CircleRequestManagement;

namespace MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;

public interface ICircleRequestReviewService
{
    Task<Result<IReadOnlyList<CircleRequestSummaryDto>>> GetQueueAsync(CancellationToken cancellationToken = default);
    Task<Result<CircleRequestResponseDto>> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task<Result<CircleRequestResponseDto>> ApproveAsync(Guid adminId, Guid requestId, CancellationToken cancellationToken = default);
    Task<Result<CircleRequestResponseDto>> RejectAsync(Guid adminId, Guid requestId, DecisionReasonDto dto, CancellationToken cancellationToken = default);
    Task<Result<CircleRequestResponseDto>> RequestModificationAsync(Guid adminId, Guid requestId, DecisionReasonDto dto, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AuditLogResponseDto>>> GetAuditAsync(Guid requestId, CancellationToken cancellationToken = default);
}
