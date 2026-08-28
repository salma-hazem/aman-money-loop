using MonyLoop.Domain.Constants.Onboarding___Member_Ledger;
using MonyLoop.Domain.Entities.Onboarding___Member_Ledger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Domain.Interfaces.OnboardingMemberLedger
{
    public interface IOnboardingCaseRepository : IGenericRepository<OnboardingCase>
    {
        Task<OnboardingCase?> GetByIdWithDocumentsAsync(Guid onboardingCaseId, CancellationToken ct = default);
        Task<OnboardingCase?> GetByMemberShipAgreementIdAsync(Guid memberShipAgreementId, CancellationToken ct = default);
        Task<IEnumerable<OnboardingCase>> GetByOrganizerIdAsync(Guid organizerId, CancellationToken ct = default);
        Task<IEnumerable<OnboardingCase>> GetByStatusAsync(OnboardingCaseStatus status, CancellationToken ct = default);
        Task<bool> ExistsForAgreementAsync(Guid memberShipAgreementId, CancellationToken ct = default);

        Task<(IReadOnlyList<OnboardingCase> Items, int TotalCount)> GetByOrganizerIdPagedAsync(
            Guid organizerId, int pageNumber, int pageSize, CancellationToken ct = default);

        Task<(IReadOnlyList<OnboardingCase> Items, int TotalCount)> GetByStatusPagedAsync(
            OnboardingCaseStatus status, int pageNumber, int pageSize, CancellationToken ct = default);

        Task<OnboardingCase?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

    }
}
