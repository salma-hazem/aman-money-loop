using Microsoft.EntityFrameworkCore;
using Mony_Loop.Domain.Constants;
using Mony_Loop.Domain.Entities.CircleRequestManagement;
using Mony_Loop.Domain.Interfaces.CircleRequestManagement;
using Mony_Loop.Infrastructure.Data;

namespace Mony_Loop.Infrastructure.Repositories.CircleRequestManagement;

public sealed class CircleSlotRepository : ICircleSlotRepository
{
    private readonly MonyLoopDbContext _context;

    public CircleSlotRepository(MonyLoopDbContext context)
    {
        _context = context;
    }

    public Task<CircleSlot?> GetByIdAsync(
        Guid circleSlotId,
        CancellationToken cancellationToken = default)
    {
        return _context.CircleSlots.FirstOrDefaultAsync(
            slot => slot.CircleSlotId == circleSlotId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<CircleSlot>> GetByCircleIdAsync(
        Guid circleId,
        CancellationToken cancellationToken = default)
    {
        return await _context.CircleSlots
            .AsNoTracking()
            .Where(slot => slot.CircleId == circleId)
            .OrderBy(slot => slot.SlotNumber)
            .ToListAsync(cancellationToken);
    }

    public Task<CircleSlot?> GetVacantAsync(
        Guid circleId,
        int? slotNumber = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<CircleSlot> query = _context.CircleSlots
            .Where(slot =>
                slot.CircleId == circleId &&
                slot.MemberLedgerId == null &&
                slot.Status == CircleSlotStatus.Vacant);

        if (slotNumber.HasValue)
        {
            query = query.Where(slot => slot.SlotNumber == slotNumber.Value);
        }

        return query
            .OrderBy(slot => slot.SlotNumber)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<CircleSlot?> GetByMemberLedgerIdAsync(
        Guid memberLedgerId,
        CancellationToken cancellationToken = default)
    {
        return _context.CircleSlots.FirstOrDefaultAsync(
            slot => slot.MemberLedgerId == memberLedgerId,
            cancellationToken);
    }

    public Task AddRangeAsync(
        IEnumerable<CircleSlot> slots,
        CancellationToken cancellationToken = default)
    {
        return _context.CircleSlots.AddRangeAsync(slots, cancellationToken);
    }
}
