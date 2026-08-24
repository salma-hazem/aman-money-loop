using Microsoft.EntityFrameworkCore;
using MonyLoop.Domain.Entities.Onboarding___Member_Ledger;
using MonyLoop.Infrastructure.Data;
using MonyLoop.Domain.Interfaces.OnboardingMemberLedger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Infrastructure.Repositories.OnboardingMemberLedger
{
    public class MemberLedgerRepository : GenericRepository<MemberLedger>, IMemberLedgerRepository
    {
        private readonly MonyLoopDbContext _dbcontext;

        public MemberLedgerRepository(MonyLoopDbContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<bool> ExistsForUserAsync(Guid userId, CancellationToken ct = default)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            return await _dbcontext.MemberLedgers
                .AsNoTracking()
                .AnyAsync(x => x.UserId == userId, ct);
        }

        public async Task<MemberLedger?> GetByIdWithSlotAsync(Guid memberLedgerId, CancellationToken ct = default)
        {
            if (memberLedgerId == Guid.Empty)
                throw new ArgumentException("Invalid member ledger ID", nameof(memberLedgerId));

            return await _dbcontext.MemberLedgers
                .Include(x => x.CircleSlot)
                .FirstOrDefaultAsync(x => x.MemberLedgerId == memberLedgerId, ct);
        }

        public async Task<MemberLedger?> GetByOnboardingCaseIdAsync(Guid onboardingCaseId, CancellationToken ct = default)
        {
            if (onboardingCaseId == Guid.Empty)
                throw new ArgumentException("Invalid onboarding case ID", nameof(onboardingCaseId));

            return await _dbcontext.MemberLedgers
                .FirstOrDefaultAsync(x => x.OnboardingCaseId == onboardingCaseId, ct);
        }

        public async Task<MemberLedger?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            return await _dbcontext.MemberLedgers
                .FirstOrDefaultAsync(x => x.UserId == userId, ct);
        }

        public async Task<List<MemberLedger>> GetAllWithDetailsAsync(CancellationToken ct = default)
        {
            return await _dbcontext.MemberLedgers
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.CircleSlot!)
                    .ThenInclude(slot => slot.Circle!)
                        .ThenInclude(circle => circle.CircleRequest)
                .OrderByDescending(x => x.ActivatedAt)
                .ToListAsync(ct);
        }

        public async Task<List<MemberLedger>> GetByOrganizerIdAsync(Guid organizerId,CancellationToken ct = default)
        {
            if (organizerId == Guid.Empty)
                throw new ArgumentException("Invalid organizer ID", nameof(organizerId));

            return await _dbcontext.MemberLedgers
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.CircleSlot!)
                    .ThenInclude(slot => slot.Circle!)
                        .ThenInclude(circle => circle.CircleRequest)
                .Where(x =>
                    x.CircleSlot != null &&
                    x.CircleSlot.Circle != null &&
                    x.CircleSlot.Circle.CircleRequest != null &&
                    x.CircleSlot.Circle.CircleRequest.CreatedByOrganizerId == organizerId)
                .OrderByDescending(x => x.ActivatedAt)
                .ToListAsync(ct);
        }
    }
}
