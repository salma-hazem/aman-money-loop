using AutoMapper;
using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.CircleRequestManagement;
using MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.CircleRequestManagement;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.CircleRequestManagement;

namespace MonyLoop.Application.Services.CircleRequestManagement;

public sealed class CircleRegistryService : ICircleRegistryService, ISlotAssignmentService
{
    private readonly ICircleRepository _circleRepository;
    private readonly ICircleSlotRepository _slotRepository;
    private readonly IMarketplaceListingRepository _listingRepository;
    private readonly IAuditLogRepository _auditRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public CircleRegistryService(
        ICircleRepository circleRepository,
        ICircleSlotRepository slotRepository,
        IMarketplaceListingRepository listingRepository,
        IAuditLogRepository auditRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        TimeProvider timeProvider)
    {
        _circleRepository = circleRepository;
        _slotRepository = slotRepository;
        _listingRepository = listingRepository;
        _auditRepository = auditRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    public async Task<Result<IReadOnlyList<CircleResponseDto>>> GetAllAsync(
        CircleStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var circles = await _circleRepository.GetAllAsync(status, cancellationToken);
        return Result<IReadOnlyList<CircleResponseDto>>.Ok(
            _mapper.Map<IReadOnlyList<CircleResponseDto>>(circles));
    }

    public async Task<Result<CircleResponseDto>> GetByIdAsync(
        Guid circleId,
        CancellationToken cancellationToken = default)
    {
        var circle = await _circleRepository.GetDetailsByIdAsync(circleId, cancellationToken);
        return circle is null
            ? CircleRequestErrors.CircleNotFound
            : _mapper.Map<CircleResponseDto>(circle);
    }

    public async Task<Result<IReadOnlyList<CircleSlotResponseDto>>> GetSlotsAsync(
        Guid circleId,
        CancellationToken cancellationToken = default)
    {
        if (await _circleRepository.GetByIdAsync(circleId, cancellationToken) is null)
        {
            return CircleRequestErrors.CircleNotFound;
        }

        var slots = await _slotRepository.GetByCircleIdAsync(circleId, cancellationToken);
        return Result<IReadOnlyList<CircleSlotResponseDto>>.Ok(
            _mapper.Map<IReadOnlyList<CircleSlotResponseDto>>(slots));
    }

    public async Task<Result<CircleSlotResponseDto>> AssignMemberLedgerAsync(
        Guid actorUserId,
        Guid circleId,
        int slotNumber,
        Guid memberLedgerId,
        CancellationToken cancellationToken = default)
    {
        var circle = await _circleRepository.GetByIdAsync(circleId, cancellationToken);
        if (circle is null)
        {
            return CircleRequestErrors.CircleNotFound;
        }

        if (circle.Status == CircleStatus.Closed)
        {
            return Error.Validation("Circle.Closed", "A closed circle cannot accept slot assignments.");
        }

        if (await _slotRepository.GetByMemberLedgerIdAsync(memberLedgerId, cancellationToken) is not null)
        {
            return Error.Validation("CircleSlot.LedgerAlreadyAssigned", "This member ledger is already assigned to a circle slot.");
        }

        var slot = await _slotRepository.GetVacantAsync(circleId, slotNumber, cancellationToken);
        if (slot is null)
        {
            return Error.Validation("CircleSlot.NotVacant", "The requested slot does not exist or is not vacant.");
        }

        var now = UtcNow();
        slot.MemberLedgerId = memberLedgerId;
        slot.Status = CircleSlotStatus.Assigned;
        slot.AssignedAt = now;
        slot.VacatedAt = null;
        circle.FilledCount = Math.Min(circle.ApprovedSlots, circle.FilledCount + 1);

        if (circle.FilledCount == circle.ApprovedSlots)
        {
            circle.Status = CircleStatus.Filled;
            var listing = await _listingRepository.GetByCircleIdAsync(circleId, cancellationToken);
            if (listing is not null)
            {
                listing.ListingStatus = MarketplaceListingStatus.Completed;
            }
        }

        await AddSlotAuditAsync(slot, "Assigned", actorUserId, CircleSlotStatus.Vacant, CircleSlotStatus.Assigned, $"Member ledger {memberLedgerId} assigned to slot {slotNumber}.", now, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CircleSlotResponseDto>(slot);
    }

    public async Task<Result<CircleSlotResponseDto>> VacateSlotAsync(
        Guid actorUserId,
        Guid circleId,
        int slotNumber,
        CancellationToken cancellationToken = default)
    {
        var circle = await _circleRepository.GetByIdAsync(circleId, cancellationToken);
        if (circle is null)
        {
            return CircleRequestErrors.CircleNotFound;
        }

        var slot = await _slotRepository.GetByCircleAndSlotNumberAsync(circleId, slotNumber, cancellationToken);
        if (slot is null)
        {
            return CircleRequestErrors.SlotNotFound;
        }

        if (slot.Status != CircleSlotStatus.Assigned || !slot.MemberLedgerId.HasValue)
        {
            return Error.Validation("CircleSlot.NotAssigned", "Only an assigned slot can be vacated.");
        }

        var previousMemberLedgerId = slot.MemberLedgerId.Value;
        var now = UtcNow();
        slot.MemberLedgerId = null;
        slot.Status = CircleSlotStatus.Vacant;
        slot.AssignedAt = null;
        slot.VacatedAt = now;
        circle.FilledCount = Math.Max(0, circle.FilledCount - 1);
        circle.Status = CircleStatus.Open;

        await AddSlotAuditAsync(slot, "Vacated", actorUserId, CircleSlotStatus.Assigned, CircleSlotStatus.Vacant, $"Slot {slotNumber} vacated from member ledger {previousMemberLedgerId}.", now, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CircleSlotResponseDto>(slot);
    }

    private Task AddSlotAuditAsync(
        CircleSlot slot,
        string action,
        Guid actorUserId,
        CircleSlotStatus oldStatus,
        CircleSlotStatus newStatus,
        string description,
        DateTime now,
        CancellationToken cancellationToken)
    {
        return _auditRepository.AddAsync(
            AuditLogFactory.Create(
                nameof(CircleSlot),
                slot.CircleSlotId,
                action,
                actorUserId,
                oldStatus.ToString(),
                newStatus.ToString(),
                description,
                now),
            cancellationToken);
    }

    private DateTime UtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
}
