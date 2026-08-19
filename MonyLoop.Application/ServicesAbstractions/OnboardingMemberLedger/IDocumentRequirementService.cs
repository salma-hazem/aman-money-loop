using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger
{
    public interface IDocumentRequirementService
    {
        Task<Result<IEnumerable<DocumentRequirementResponseDto>>> GetActiveOrderedAsync(CancellationToken ct = default);
        Task<Result<IEnumerable<DocumentRequirementResponseDto>>> GetRequiredOnlyAsync(CancellationToken ct = default);
    }
}
