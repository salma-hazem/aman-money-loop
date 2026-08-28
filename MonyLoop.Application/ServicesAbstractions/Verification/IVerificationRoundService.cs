using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MonyLoop.Application.DTOs.Verification;

namespace MonyLoop.Application.ServicesAbstractions.Verification
{
    public interface IVerificationRoundService
    {
        Task<VerificationRoundResponseDto> CreateRoundAsync(CreateVerificationRoundDto dto, CancellationToken ct = default);
        Task<VerificationRoundResponseDto?> GetRoundByIdAsync(Guid verificationRoundId, CancellationToken ct = default);
        Task<IReadOnlyList<VerificationRoundResponseDto>> GetRoundsByCircleIdAsync(Guid circleId, CancellationToken ct = default);
        Task<VerificationRoundResponseDto?> UpdateRoundAsync(Guid verificationRoundId, UpdateVerificationRoundDto dto, CancellationToken ct = default);
    }
}