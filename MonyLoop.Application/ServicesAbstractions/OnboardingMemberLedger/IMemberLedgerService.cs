using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger
{
    public interface IMemberLedgerService
    {
        Task<Result<MemberLedgerResponseDto>> ActivateAsync(MemberLedgerRequestDto request, Guid activatedByAdminId, CancellationToken ct = default);
        Task<Result<MemberLedgerResponseDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

        Task<Result<List<MemberLedgerResponseDto>>> GetAllAsync(CancellationToken ct = default);

        Task<Result<List<MemberLedgerResponseDto>>> GetByOrganizerIdAsync(Guid organizerId,CancellationToken ct = default);
    }
}
