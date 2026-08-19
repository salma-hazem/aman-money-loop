using Microsoft.EntityFrameworkCore;
using MonyLoop.Domain.Constants.Onboarding___Member_Ledger;
using MonyLoop.Domain.Entities.Onboarding___Member_Ledger;
using MonyLoop.Infrastructure.Data;
using MonyLoop.Domain.Interfaces.OnboardingMemberLedger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Infrastructure.Repositories.OnboardingMemberLedger
{
    public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
    {
        private readonly MonyLoopDbContext _dbcontext;
        public DocumentRepository(MonyLoopDbContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<bool> AllRequiredDocumentsVerifiedAsync(Guid onboardingCaseId, CancellationToken ct = default)
        {
            if (onboardingCaseId == Guid.Empty)
                throw new ArgumentException("Invalid onboarding case ID", nameof(onboardingCaseId));

            var reqeiredRequirementsIds = await _dbcontext.DocumentRequirements
                .Where(x => x.IsRequired && x.IsActive)
                .Select(x => x.DocumentRequirementId)
                .ToListAsync(ct);

            if (!reqeiredRequirementsIds.Any())
                return true;

            var verifiedRequirmentsIds = await _dbcontext.Documents
                .Where(x => x.OnboardingCaseId == onboardingCaseId
                && x.Status == DocumentStatus.Approved
                && reqeiredRequirementsIds.Contains(x.DocumentRequirementId))
                .Select(x => x.DocumentRequirementId)
                .Distinct()
                .ToListAsync(ct);

            return reqeiredRequirementsIds.All(id => verifiedRequirmentsIds.Contains(id));

        }

        public async Task<Document?> GetByCaseAndRequirementAsync(Guid onboardingCaseId, Guid documentRequirementId, CancellationToken ct = default)
        {
            if (onboardingCaseId == Guid.Empty)
                throw new ArgumentException("Invalid onboarding case ID", nameof(onboardingCaseId));

            if (documentRequirementId == Guid.Empty)
                throw new ArgumentException("Invalid document requirement ID", nameof(documentRequirementId));

            return await _dbcontext.Documents
                .FirstOrDefaultAsync(x => x.OnboardingCaseId == onboardingCaseId && x.DocumentRequirementId == documentRequirementId, ct);
        }

        public async Task<IEnumerable<Document>> GetByOnboardingCaseIdAsync(Guid OnboardingCaseId, CancellationToken ct = default)
        {
            if (OnboardingCaseId == Guid.Empty)
                throw new ArgumentException("Invalid onboarding case ID", nameof(OnboardingCaseId));

            return await _dbcontext.Documents
                .Where(x => x.OnboardingCaseId == OnboardingCaseId)
                .AsNoTracking()
                .ToListAsync(ct);

        }

        public async Task<IEnumerable<Document>> GetPendingReviewAsync(CancellationToken ct = default)
        {
            return await _dbcontext.Documents
                .Where(x => x.Status == DocumentStatus.Pending)
                .AsNoTracking()
                .ToListAsync(ct);


        }
    }
}
