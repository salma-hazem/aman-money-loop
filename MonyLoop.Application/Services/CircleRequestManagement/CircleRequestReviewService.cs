using AutoMapper;
using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.CircleRequestManagement;
using MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.CircleRequestManagement;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.CircleRequestManagement;

namespace MonyLoop.Application.Services.CircleRequestManagement;

public sealed class CircleRequestReviewService : ICircleRequestReviewService
{
    private readonly ICircleRequestRepository _requestRepository;
    private readonly ICircleRepository _circleRepository;
    private readonly ICircleSlotRepository _slotRepository;
    private readonly IAuditLogRepository _auditRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICircleRequestNotificationService _notifications;
    private readonly IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public CircleRequestReviewService(
        ICircleRequestRepository requestRepository,
        ICircleRepository circleRepository,
        ICircleSlotRepository slotRepository,
        IAuditLogRepository auditRepository,
        IUnitOfWork unitOfWork,
        ICircleRequestNotificationService notifications,
        IMapper mapper,
        TimeProvider timeProvider)
    {
        _requestRepository = requestRepository;
        _circleRepository = circleRepository;
        _slotRepository = slotRepository;
        _auditRepository = auditRepository;
        _unitOfWork = unitOfWork;
        _notifications = notifications;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    public async Task<Result<IReadOnlyList<CircleRequestSummaryDto>>> GetQueueAsync(
        CancellationToken cancellationToken = default)
    {
        var requests = await _requestRepository.GetByStatusAsync(CircleRequestStatus.Submitted, cancellationToken);
        return Result<IReadOnlyList<CircleRequestSummaryDto>>.Ok(
            _mapper.Map<IReadOnlyList<CircleRequestSummaryDto>>(requests));
    }

    public async Task<Result<CircleRequestResponseDto>> GetByIdAsync(
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        var request = await _requestRepository.GetByIdAsync(requestId, cancellationToken);
        return request is null
            ? CircleRequestErrors.RequestNotFound
            : _mapper.Map<CircleRequestResponseDto>(request);
    }

    public async Task<Result<CircleRequestResponseDto>> ApproveAsync(
        Guid adminId,
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        var request = await _requestRepository.GetByIdAsync(requestId, cancellationToken);
        if (request is null)
        {
            return CircleRequestErrors.RequestNotFound;
        }

        if (request.RequestStatus != CircleRequestStatus.Submitted)
        {
            return CircleRequestErrors.InvalidTransition(request.RequestStatus.ToString(), "approve");
        }

        if (request.CircleType == CircleType.NewCircle)
        {
            if (await _circleRepository.ExistsForRequestAsync(request.RequestId, cancellationToken))
            {
                return Error.Validation("CircleRequest.AlreadyApproved", "A circle already exists for this request.");
            }

            var circle = new Circle
            {
                CircleId = Guid.NewGuid(),
                RequestId = request.RequestId,
                ApprovedSlots = request.NumberOfSlots,
                FilledCount = 0,
                Amount = request.ContributionAmount,
                Duration = request.Duration,
                Status = CircleStatus.Open
            };

            var slots = Enumerable.Range(1, request.NumberOfSlots)
                .Select(slotNumber => new CircleSlot
                {
                    CircleSlotId = Guid.NewGuid(),
                    CircleId = circle.CircleId,
                    SlotNumber = slotNumber,
                    Status = CircleSlotStatus.Vacant
                })
                .ToList();

            await _circleRepository.AddAsync(circle, cancellationToken);
            await _slotRepository.AddRangeAsync(slots, cancellationToken);
        }
        else
        {
            var targetValidation = await ValidateReplacementAtReviewAsync(request, cancellationToken);
            if (targetValidation.IsFailure)
            {
                return Result<CircleRequestResponseDto>.Fail(targetValidation.Errors.ToList());
            }
        }

        await CompleteDecisionAsync(request, adminId, CircleRequestStatus.Approved, null, "Approved", "Circle request approved.", cancellationToken);
        return _mapper.Map<CircleRequestResponseDto>(request);
    }

    public Task<Result<CircleRequestResponseDto>> RejectAsync(
        Guid adminId,
        Guid requestId,
        DecisionReasonDto dto,
        CancellationToken cancellationToken = default)
    {
        return CompleteReasonedDecisionAsync(adminId, requestId, dto, CircleRequestStatus.Rejected, "Rejected", cancellationToken);
    }

    public Task<Result<CircleRequestResponseDto>> RequestModificationAsync(
        Guid adminId,
        Guid requestId,
        DecisionReasonDto dto,
        CancellationToken cancellationToken = default)
    {
        return CompleteReasonedDecisionAsync(adminId, requestId, dto, CircleRequestStatus.ModificationRequested, "ModificationRequested", cancellationToken);
    }

    public async Task<Result<IReadOnlyList<AuditLogResponseDto>>> GetAuditAsync(
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        if (!await _requestRepository.ExistsAsync(requestId, cancellationToken))
        {
            return CircleRequestErrors.RequestNotFound;
        }

        var history = await _auditRepository.GetByEntityAsync(nameof(CircleRequest), requestId, cancellationToken);
        return Result<IReadOnlyList<AuditLogResponseDto>>.Ok(
            _mapper.Map<IReadOnlyList<AuditLogResponseDto>>(history));
    }

    private async Task<Result<CircleRequestResponseDto>> CompleteReasonedDecisionAsync(
        Guid adminId,
        Guid requestId,
        DecisionReasonDto dto,
        CircleRequestStatus newStatus,
        string action,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Reason))
        {
            return Error.Validation("CircleRequest.DecisionReasonRequired", "A decision reason is required.");
        }

        var request = await _requestRepository.GetByIdAsync(requestId, cancellationToken);
        if (request is null)
        {
            return CircleRequestErrors.RequestNotFound;
        }

        if (request.RequestStatus != CircleRequestStatus.Submitted)
        {
            return CircleRequestErrors.InvalidTransition(request.RequestStatus.ToString(), action.ToLowerInvariant());
        }

        var reason = dto.Reason.Trim();
        await CompleteDecisionAsync(request, adminId, newStatus, reason, action, reason, cancellationToken);
        return _mapper.Map<CircleRequestResponseDto>(request);
    }

    private async Task CompleteDecisionAsync(
        CircleRequest request,
        Guid adminId,
        CircleRequestStatus newStatus,
        string? reason,
        string action,
        string description,
        CancellationToken cancellationToken)
    {
        var oldStatus = request.RequestStatus;
        var now = UtcNow();
        request.RequestStatus = newStatus;
        request.ReviewedByAdminId = adminId;
        request.ReviewedAt = now;
        request.DecisionReason = reason;

        await _auditRepository.AddAsync(
            AuditLogFactory.Create(
                nameof(CircleRequest),
                request.RequestId,
                action,
                adminId,
                oldStatus.ToString(),
                newStatus.ToString(),
                description,
                now),
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notifications.NotifyDecisionAsync(request, cancellationToken);
    }

    private async Task<Result> ValidateReplacementAtReviewAsync(
        CircleRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.ExistingCircleId.HasValue || !request.VacantSlotNumber.HasValue)
        {
            return Result.Fail(CircleRequestErrors.InvalidReplacement("Replacement target data is incomplete."));
        }

        var circle = await _circleRepository.GetByIdAsync(request.ExistingCircleId.Value, cancellationToken);
        if (circle is null || circle.Status == CircleStatus.Closed)
        {
            return Result.Fail(CircleRequestErrors.InvalidReplacement("The target circle does not exist or is closed."));
        }

        var slot = await _slotRepository.GetVacantAsync(circle.CircleId, request.VacantSlotNumber.Value, cancellationToken);
        if (slot is null)
        {
            return Result.Fail(CircleRequestErrors.InvalidReplacement("The requested replacement slot is no longer vacant."));
        }

        return Result.Ok();
    }

    private DateTime UtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
}
