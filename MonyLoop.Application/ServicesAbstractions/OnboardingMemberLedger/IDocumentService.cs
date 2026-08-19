using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger
{
    public interface IDocumentService
    {
        Task<Result<DocumentResponseDto>> UploadAsync(DocumentRequestDto request, CancellationToken ct = default);
        Task<Result<IEnumerable<DocumentResponseDto>>> GetByOnboardingCaseIdAsync(Guid onboardingCaseId, CancellationToken ct = default);
        Task<Result<IEnumerable<DocumentResponseDto>>> GetPendingReviewAsync(CancellationToken ct = default);
        Task<Result<DocumentResponseDto>> ReviewAsync(DocumentReviewRequestDto request, CancellationToken ct = default);
    }
}
