using Microsoft.EntityFrameworkCore;
using MonyLoop.Domain.Constants.Onboarding___Member_Ledger;
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
    public class OnboardingCaseRepository : GenericRepository<OnboardingCase>, IOnboardingCaseRepository
    {
        private readonly MonyLoopDbContext _dbcontext;

        public OnboardingCaseRepository(MonyLoopDbContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<bool> ExistsForAgreementAsync(Guid memberShipAgreementId, CancellationToken ct = default)
        {
            if (memberShipAgreementId == Guid.Empty)
                throw new ArgumentException("Invalid membership agreement ID", nameof(memberShipAgreementId));

            return await _dbcontext.OnboardingCases
                .AsNoTracking()
                .AnyAsync(c => c.MembershipAgreementId == memberShipAgreementId, ct);
        }

        public async Task<OnboardingCase?> GetByIdWithDocumentsAsync(Guid onboardingCaseId, CancellationToken ct = default)
        {
            if (onboardingCaseId == Guid.Empty)
                throw new ArgumentException("Invalid onboarding case ID", nameof(onboardingCaseId));

            return await _dbcontext.OnboardingCases
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.OnboardingCaseId == onboardingCaseId, ct);
        }

        public async Task<OnboardingCase?> GetByMemberShipAgreementIdAsync(Guid memberShipAgreementId, CancellationToken ct = default)
        {
            if (memberShipAgreementId == Guid.Empty)
                throw new ArgumentException("Invalid membership agreement ID", nameof(memberShipAgreementId));

            return await _dbcontext.OnboardingCases
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.MembershipAgreementId == memberShipAgreementId, ct);
        }

        public async Task<IEnumerable<OnboardingCase>> GetByOrganizerIdAsync(Guid organizerId, CancellationToken ct = default)
        {
            if (organizerId == Guid.Empty)
                throw new ArgumentException("Invalid organizer ID", nameof(organizerId));

            return await _dbcontext.OnboardingCases
                .AsNoTracking()
                .Where(c => c.OrganizerId == organizerId)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<OnboardingCase>> GetByStatusAsync(OnboardingCaseStatus status, CancellationToken ct = default)
        {
            return await _dbcontext.OnboardingCases
                .AsNoTracking()
                .Where(c => c.FinalStatus == status)
                .ToListAsync(ct);
        }
    }
}
