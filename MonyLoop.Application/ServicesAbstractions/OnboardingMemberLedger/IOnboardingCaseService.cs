using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Domain.Constants.Onboarding___Member_Ledger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger
{
    public interface IOnboardingCaseService
    {
        Task<Result<OnboardingCaseResponseDto>> CreateAsync(OnboardingCaseRequestDto request, CancellationToken ct = default);
        Task<Result<OnboardingCaseResponseDto>> GetByIdAsync(Guid onboardingCaseId, CancellationToken ct = default);
        Task<Result<OnboardingCaseResponseDto>> GetByIdWithDocumentsAsync(Guid onboardingCaseId, CancellationToken ct = default);
        Task<Result<IEnumerable<OnboardingCaseResponseDto>>> GetByOrganizerIdAsync(Guid organizerId, CancellationToken ct = default);
        Task<Result<IEnumerable<OnboardingCaseResponseDto>>> GetByStatusAsync(OnboardingCaseStatus status, CancellationToken ct = default);
        Task<Result> MarkDocumentsVerifiedAsync(Guid onboardingCaseId, CancellationToken ct = default);
        Task<Result> RecalculateAndUpdateStatusAsync(Guid onboardingCaseId, CancellationToken ct = default);
        Task<Result> MarkActivatedAsync(Guid onboardingCaseId, Guid activatedByAdminId, CancellationToken ct = default);

    }
}
