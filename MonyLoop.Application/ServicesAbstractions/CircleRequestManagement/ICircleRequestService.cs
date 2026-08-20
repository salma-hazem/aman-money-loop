using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.CircleRequestManagement;

namespace MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;

public interface ICircleRequestService
{
    Task<Result<CircleRequestResponseDto>> CreateNewAsync(Guid organizerId, CreateNewCircleRequestDto dto, CancellationToken cancellationToken = default);
    Task<Result<CircleRequestResponseDto>> CreateReplacementAsync(Guid organizerId, CreateReplacementCircleRequestDto dto, CancellationToken cancellationToken = default);
    Task<Result<CircleRequestResponseDto>> UpdateNewAsync(Guid organizerId, Guid requestId, UpdateNewCircleRequestDto dto, CancellationToken cancellationToken = default);
    Task<Result<CircleRequestResponseDto>> UpdateReplacementAsync(Guid organizerId, Guid requestId, UpdateReplacementCircleRequestDto dto, CancellationToken cancellationToken = default);
    Task<Result<CircleRequestResponseDto>> SubmitAsync(Guid organizerId, Guid requestId, CancellationToken cancellationToken = default);
    Task<Result<CircleRequestResponseDto>> PublishAsync(Guid organizerId, Guid requestId, CancellationToken cancellationToken = default);
    Task<Result<CircleRequestResponseDto>> CancelAsync(Guid organizerId, Guid requestId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<CircleRequestSummaryDto>>> GetMineAsync(Guid organizerId, CancellationToken cancellationToken = default);
    Task<Result<CircleRequestResponseDto>> GetByIdAsync(Guid organizerId, Guid requestId, CancellationToken cancellationToken = default);
}
