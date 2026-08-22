using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MonyLoop.Application.DTOs.Verification;

namespace MonyLoop.Application.ServicesAbstractions.Verification
{
    public interface IVerificationScheduleService
    {
        Task<VerificationScheduleResponseDto> CreateScheduleAsync(CreateVerificationScheduleDto dto, CancellationToken ct = default);
        Task<VerificationScheduleResponseDto?> GetScheduleByIdAsync(Guid verificationScheduleId, CancellationToken ct = default);
        Task<IReadOnlyList<VerificationScheduleResponseDto>> GetSchedulesByApplicationIdAsync(Guid applicationId, CancellationToken ct = default);
        Task UpdateScheduleStatusAsync(Guid verificationScheduleId, UpdateVerificationScheduleDto dto, CancellationToken ct = default);
    }
}