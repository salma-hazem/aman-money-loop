using MonyLoop.Domain.Entities.Onboarding___Member_Ledger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Domain.Interfaces.OnboardingMemberLedger
{
    public interface IMemberLedgerRepository : IGenericRepository<MemberLedger>
    {
        Task<MemberLedger?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<MemberLedger?> GetByOnboardingCaseIdAsync(Guid onboardingCaseId, CancellationToken ct = default);
        Task<MemberLedger?> GetByIdWithSlotAsync(Guid memberLedgerId, CancellationToken ct = default);
        Task<bool> ExistsForUserAsync(Guid userId, CancellationToken ct = default);

        Task<List<MemberLedger>> GetAllWithDetailsAsync(CancellationToken ct = default);

        Task<List<MemberLedger>> GetByOrganizerIdAsync(Guid organizerId,CancellationToken ct = default);
    }
}
