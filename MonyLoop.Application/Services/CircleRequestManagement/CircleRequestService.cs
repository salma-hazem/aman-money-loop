using AutoMapper;
using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.CircleRequestManagement;
using MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.CircleRequestManagement;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.CircleRequestManagement;

namespace MonyLoop.Application.Services.CircleRequestManagement;

public sealed class CircleRequestService : ICircleRequestService
{
    private readonly ICircleRequestRepository _requestRepository;
    private readonly ICircleRepository _circleRepository;
    private readonly ICircleSlotRepository _slotRepository;
    private readonly IMarketplaceListingRepository _listingRepository;
    private readonly IAuditLogRepository _auditRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICircleRequestNotificationService _notifications;
    private readonly IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public CircleRequestService(
        ICircleRequestRepository requestRepository,
        ICircleRepository circleRepository,
        ICircleSlotRepository slotRepository,
        IMarketplaceListingRepository listingRepository,
        IAuditLogRepository auditRepository,
        IUnitOfWork unitOfWork,
        ICircleRequestNotificationService notifications,
        IMapper mapper,
        TimeProvider timeProvider)
    {
        _requestRepository = requestRepository;
        _circleRepository = circleRepository;
        _slotRepository = slotRepository;
        _listingRepository = listingRepository;
        _auditRepository = auditRepository;
        _unitOfWork = unitOfWork;
        _notifications = notifications;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    public async Task<Result<CircleRequestResponseDto>> CreateNewAsync(
        Guid organizerId,
        CreateNewCircleRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.CircleTitle) || string.IsNullOrWhiteSpace(dto.ShortJustification))
        {
            return Error.Validation("CircleRequest.RequiredFields", "Title and justification are required.");
        }

        var now = UtcNow();
        var request = new CircleRequest
        {
            RequestId = Guid.NewGuid(),
            CreatedByOrganizerId = organizerId,
            CircleTitle = dto.CircleTitle.Trim(),
            CircleType = CircleType.NewCircle,
            ContributionAmount = dto.ContributionAmount,
            Duration = dto.Duration,
            NumberOfSlots = dto.NumberOfSlots,
            ShortJustification = dto.ShortJustification.Trim(),
            RequestStatus = CircleRequestStatus.Draft,
            CreatedAt = now
        };

        await _requestRepository.AddAsync(request, cancellationToken);
        await AddRequestAuditAsync(request, "DraftCreated", organizerId, null, request.RequestStatus, "New-circle draft created.", now, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CircleRequestResponseDto>(request);
    }

    public async Task<Result<CircleRequestResponseDto>> CreateReplacementAsync(
        Guid organizerId,
        CreateReplacementCircleRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await ValidateReplacementTargetAsync(dto.ExistingCircleId, dto.VacantSlotNumber, null, cancellationToken);
        if (validation.IsFailure)
        {
            return Result<CircleRequestResponseDto>.Fail(validation.Errors.ToList());
        }

        var circle = validation.Value;
        var now = UtcNow();
        var request = new CircleRequest
        {
            RequestId = Guid.NewGuid(),
            ExistingCircleId = circle.CircleId,
            CreatedByOrganizerId = organizerId,
            CircleTitle = circle.CircleRequest!.CircleTitle,
            CircleType = CircleType.Replacement,
            ContributionAmount = circle.Amount,
            Duration = circle.Duration,
            NumberOfSlots = 1,
            ShortJustification = NormalizeOptional(dto.ShortJustification),
            RequestStatus = CircleRequestStatus.Draft,
            VacantSlotNumber = dto.VacantSlotNumber,
            CreatedAt = now
        };

        await _requestRepository.AddAsync(request, cancellationToken);
        await AddRequestAuditAsync(request, "DraftCreated", organizerId, null, request.RequestStatus, "Replacement-circle draft created.", now, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CircleRequestResponseDto>(request);
    }

    public async Task<Result<CircleRequestResponseDto>> UpdateNewAsync(
        Guid organizerId,
        Guid requestId,
        UpdateNewCircleRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        var owned = await GetOwnedRequestAsync(organizerId, requestId, cancellationToken);
        if (owned.IsFailure)
        {
            return Result<CircleRequestResponseDto>.Fail(owned.Errors.ToList());
        }

        var request = owned.Value;
        if (request.CircleType != CircleType.NewCircle)
        {
            return Error.Validation("CircleRequest.TypeMismatch", "This endpoint edits new-circle requests only.");
        }

        if (!CanEdit(request.RequestStatus))
        {
            return CircleRequestErrors.InvalidTransition(request.RequestStatus.ToString(), "edit");
        }

        if (string.IsNullOrWhiteSpace(dto.CircleTitle) || string.IsNullOrWhiteSpace(dto.ShortJustification))
        {
            return Error.Validation("CircleRequest.RequiredFields", "Title and justification are required.");
        }

        request.CircleTitle = dto.CircleTitle.Trim();
        request.ContributionAmount = dto.ContributionAmount;
        request.Duration = dto.Duration;
        request.NumberOfSlots = dto.NumberOfSlots;
        request.ShortJustification = dto.ShortJustification.Trim();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<CircleRequestResponseDto>(request);
    }

    public async Task<Result<CircleRequestResponseDto>> UpdateReplacementAsync(
        Guid organizerId,
        Guid requestId,
        UpdateReplacementCircleRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        var owned = await GetOwnedRequestAsync(organizerId, requestId, cancellationToken);
        if (owned.IsFailure)
        {
            return Result<CircleRequestResponseDto>.Fail(owned.Errors.ToList());
        }

        var request = owned.Value;
        if (request.CircleType != CircleType.Replacement)
        {
            return Error.Validation("CircleRequest.TypeMismatch", "This endpoint edits replacement requests only.");
        }

        if (!CanEdit(request.RequestStatus))
        {
            return CircleRequestErrors.InvalidTransition(request.RequestStatus.ToString(), "edit");
        }

        var validation = await ValidateReplacementTargetAsync(dto.ExistingCircleId, dto.VacantSlotNumber, requestId, cancellationToken);
        if (validation.IsFailure)
        {
            return Result<CircleRequestResponseDto>.Fail(validation.Errors.ToList());
        }

        var circle = validation.Value;
        request.ExistingCircleId = circle.CircleId;
        request.CircleTitle = circle.CircleRequest!.CircleTitle;
        request.ContributionAmount = circle.Amount;
        request.Duration = circle.Duration;
        request.NumberOfSlots = 1;
        request.VacantSlotNumber = dto.VacantSlotNumber;
        request.ShortJustification = NormalizeOptional(dto.ShortJustification);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<CircleRequestResponseDto>(request);
    }

    public async Task<Result<CircleRequestResponseDto>> SubmitAsync(
        Guid organizerId,
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        var owned = await GetOwnedRequestAsync(organizerId, requestId, cancellationToken);
        if (owned.IsFailure)
        {
            return Result<CircleRequestResponseDto>.Fail(owned.Errors.ToList());
        }

        var request = owned.Value;
        if (request.RequestStatus is not (CircleRequestStatus.Draft or CircleRequestStatus.ModificationRequested))
        {
            return CircleRequestErrors.InvalidTransition(request.RequestStatus.ToString(), "submit");
        }

        if (request.CircleType == CircleType.Replacement)
        {
            var replacementValidation = await ValidateReplacementTargetAsync(
                request.ExistingCircleId!.Value,
                request.VacantSlotNumber!.Value,
                request.RequestId,
                cancellationToken);

            if (replacementValidation.IsFailure)
            {
                return Result<CircleRequestResponseDto>.Fail(replacementValidation.Errors.ToList());
            }
        }

        var oldStatus = request.RequestStatus;
        var now = UtcNow();
        request.RequestStatus = CircleRequestStatus.Submitted;
        request.SubmittedAt = now;
        request.ReviewedAt = null;
        request.ReviewedByAdminId = null;
        request.DecisionReason = null;

        await AddRequestAuditAsync(request, "Submitted", organizerId, oldStatus, request.RequestStatus, "Circle request submitted for Admin review.", now, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notifications.NotifySubmittedAsync(request, cancellationToken);

        return _mapper.Map<CircleRequestResponseDto>(request);
    }

    public async Task<Result<CircleRequestResponseDto>> PublishAsync(
        Guid organizerId,
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        var owned = await GetOwnedRequestAsync(organizerId, requestId, cancellationToken);
        if (owned.IsFailure)
        {
            return Result<CircleRequestResponseDto>.Fail(owned.Errors.ToList());
        }

        var request = owned.Value;
        if (request.RequestStatus != CircleRequestStatus.Approved)
        {
            return CircleRequestErrors.InvalidTransition(request.RequestStatus.ToString(), "publish");
        }

        Circle? circle;
        if (request.CircleType == CircleType.NewCircle)
        {
            circle = await _circleRepository.GetByRequestIdAsync(request.RequestId, cancellationToken);
            if (circle is null)
            {
                return CircleRequestErrors.CircleNotFound;
            }
        }
        else
        {
            circle = await _circleRepository.GetByIdAsync(request.ExistingCircleId!.Value, cancellationToken);
            if (circle is null)
            {
                return CircleRequestErrors.CircleNotFound;
            }

            var slot = await _slotRepository.GetVacantAsync(circle.CircleId, request.VacantSlotNumber, cancellationToken);
            if (slot is null)
            {
                return CircleRequestErrors.InvalidReplacement("The requested replacement slot is no longer vacant.");
            }
        }

        var listing = await _listingRepository.GetByCircleIdAsync(circle.CircleId, cancellationToken);
        if (listing is null)
        {
            listing = new MarketplaceListing
            {
                ListingId = Guid.NewGuid(),
                CircleId = circle.CircleId,
                ListingStatus = MarketplaceListingStatus.Active
            };
            await _listingRepository.AddAsync(listing, cancellationToken);
        }
        else
        {
            listing.ListingStatus = MarketplaceListingStatus.Active;
        }

        var oldStatus = request.RequestStatus;
        request.RequestStatus = CircleRequestStatus.Published;
        circle.Status = CircleStatus.InRecruitment;
        var now = UtcNow();

        await AddRequestAuditAsync(request, "Published", organizerId, oldStatus, request.RequestStatus, "Circle request published to the marketplace.", now, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CircleRequestResponseDto>(request);
    }

    public async Task<Result<CircleRequestResponseDto>> CancelAsync(
        Guid organizerId,
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        var owned = await GetOwnedRequestAsync(organizerId, requestId, cancellationToken);
        if (owned.IsFailure)
        {
            return Result<CircleRequestResponseDto>.Fail(owned.Errors.ToList());
        }

        var request = owned.Value;
        if (request.RequestStatus is not (CircleRequestStatus.Draft or CircleRequestStatus.Submitted or CircleRequestStatus.ModificationRequested or CircleRequestStatus.Approved or CircleRequestStatus.Published))
        {
            return CircleRequestErrors.InvalidTransition(request.RequestStatus.ToString(), "cancel");
        }

        if (request.CircleType == CircleType.NewCircle)
        {
            var circle = await _circleRepository.GetByRequestIdAsync(request.RequestId, cancellationToken);
            if (circle is not null)
            {
                circle.Status = CircleStatus.Closed;
                var listing = await _listingRepository.GetByCircleIdAsync(circle.CircleId, cancellationToken);
                if (listing is not null)
                {
                    listing.ListingStatus = MarketplaceListingStatus.Cancelled;
                }
            }
        }
        else if (request.RequestStatus == CircleRequestStatus.Published)
        {
            var replacements = await _requestRepository.GetReplacementRequestsAsync(request.ExistingCircleId!.Value, cancellationToken);
            var anotherPublishedRequestExists = replacements.Any(item =>
                item.RequestId != request.RequestId &&
                item.RequestStatus == CircleRequestStatus.Published);

            if (!anotherPublishedRequestExists)
            {
                var circle = await _circleRepository.GetByIdAsync(request.ExistingCircleId.Value, cancellationToken);
                var listing = await _listingRepository.GetByCircleIdAsync(request.ExistingCircleId.Value, cancellationToken);
                if (listing is not null)
                {
                    listing.ListingStatus = MarketplaceListingStatus.Cancelled;
                }

                if (circle is not null && circle.Status == CircleStatus.InRecruitment)
                {
                    circle.Status = CircleStatus.Open;
                }
            }
        }

        var oldStatus = request.RequestStatus;
        request.RequestStatus = CircleRequestStatus.Cancelled;
        var now = UtcNow();
        await AddRequestAuditAsync(request, "Cancelled", organizerId, oldStatus, request.RequestStatus, "Circle request cancelled.", now, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CircleRequestResponseDto>(request);
    }

    public async Task<Result<IReadOnlyList<CircleRequestSummaryDto>>> GetMineAsync(
        Guid organizerId,
        CancellationToken cancellationToken = default)
    {
        var requests = await _requestRepository.GetByOrganizerIdAsync(organizerId, cancellationToken);
        return Result<IReadOnlyList<CircleRequestSummaryDto>>.Ok(
            _mapper.Map<IReadOnlyList<CircleRequestSummaryDto>>(requests));
    }

    public async Task<Result<CircleRequestResponseDto>> GetByIdAsync(
        Guid organizerId,
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        var owned = await GetOwnedRequestAsync(organizerId, requestId, cancellationToken);
        return owned.IsFailure
            ? Result<CircleRequestResponseDto>.Fail(owned.Errors.ToList())
            : _mapper.Map<CircleRequestResponseDto>(owned.Value);
    }

    private async Task<Result<CircleRequest>> GetOwnedRequestAsync(
        Guid organizerId,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var request = await _requestRepository.GetByIdAsync(requestId, cancellationToken);
        if (request is null)
        {
            return CircleRequestErrors.RequestNotFound;
        }

        if (request.CreatedByOrganizerId != organizerId)
        {
            return CircleRequestErrors.Forbidden;
        }

        return request;
    }

    private async Task<Result<Circle>> ValidateReplacementTargetAsync(
        Guid circleId,
        int slotNumber,
        Guid? excludedRequestId,
        CancellationToken cancellationToken)
    {
        var circle = await _circleRepository.GetDetailsByIdAsync(circleId, cancellationToken);
        if (circle is null)
        {
            return CircleRequestErrors.CircleNotFound;
        }

        if (circle.Status == CircleStatus.Closed)
        {
            return CircleRequestErrors.InvalidReplacement("A closed circle cannot recruit a replacement.");
        }

        var slot = await _slotRepository.GetVacantAsync(circleId, slotNumber, cancellationToken);
        if (slot is null)
        {
            return CircleRequestErrors.InvalidReplacement("The selected slot does not exist or is not vacant.");
        }

        var duplicateExists = await _requestRepository.HasActiveReplacementAsync(
            circleId,
            slotNumber,
            excludedRequestId,
            cancellationToken);

        if (duplicateExists)
        {
            return CircleRequestErrors.InvalidReplacement("An active replacement request already targets this circle and slot.");
        }

        return circle;
    }

    private Task AddRequestAuditAsync(
        CircleRequest request,
        string action,
        Guid actorUserId,
        CircleRequestStatus? oldStatus,
        CircleRequestStatus newStatus,
        string description,
        DateTime now,
        CancellationToken cancellationToken)
    {
        return _auditRepository.AddAsync(
            AuditLogFactory.Create(
                nameof(CircleRequest),
                request.RequestId,
                action,
                actorUserId,
                oldStatus?.ToString(),
                newStatus.ToString(),
                description,
                now),
            cancellationToken);
    }

    private DateTime UtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
    private static bool CanEdit(CircleRequestStatus status) => status is CircleRequestStatus.Draft or CircleRequestStatus.ModificationRequested;
    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
