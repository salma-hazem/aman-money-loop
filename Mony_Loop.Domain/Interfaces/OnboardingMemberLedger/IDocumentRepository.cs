using Mony_Loop.Domain.Entities.Onboarding___Member_Ledger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Domain.Interfaces.OnboardingMemberLedger
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
        Task<IEnumerable<Document>> GetByOnboardingCaseIdAsync(Guid OnboardingCaseId, CancellationToken ct = default);
        Task<Document?> GetByCaseAndRequirementAsync(Guid onboardingCaseId, Guid documentRequirementId, CancellationToken ct = default);
        Task<IEnumerable<Document>> GetPendingReviewAsync(CancellationToken ct = default);
        Task<bool> AllRequiredDocumentsVerifiedAsync(Guid onboardingCaseId, CancellationToken ct = default);
    }
}
