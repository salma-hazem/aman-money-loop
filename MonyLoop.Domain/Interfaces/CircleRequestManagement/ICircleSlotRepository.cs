using MonyLoop.Domain.Entities.CircleRequestManagement;

namespace MonyLoop.Domain.Interfaces.CircleRequestManagement;

public interface ICircleSlotRepository
{
    Task<CircleSlot?> GetByIdAsync(
        Guid circleSlotId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CircleSlot>> GetByCircleIdAsync(
        Guid circleId,
        CancellationToken cancellationToken = default);

    Task<CircleSlot?> GetVacantAsync(
        Guid circleId,
        int? slotNumber = null,
        CancellationToken cancellationToken = default);

    Task<CircleSlot?> GetByMemberLedgerIdAsync(
        Guid memberLedgerId,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<CircleSlot> slots,
        CancellationToken cancellationToken = default);
}
