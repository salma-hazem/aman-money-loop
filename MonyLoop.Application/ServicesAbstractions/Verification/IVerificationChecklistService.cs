using System;
using System.Threading;
using System.Threading.Tasks;
using MonyLoop.Application.DTOs.Verification;

namespace MonyLoop.Application.ServicesAbstractions.Verification
{
    public interface IVerificationChecklistService
    {
        // Submits scoring checklist, calculates composite weighted score, and updates applicant state
        Task<VerificationChecklistSubmissionResponseDto> SubmitChecklistAsync(CreateVerificationChecklistSubmissionDto dto, CancellationToken ct = default);

        // Gets submission details including individual criterion ratings
        Task<VerificationChecklistSubmissionResponseDto?> GetSubmissionByScheduleIdAsync(Guid verificationScheduleId, CancellationToken ct = default);

        // Calculates weighted composite score automatically based on active criteria weights (1-5 scale)
        Task<decimal> CalculateWeightedCompositeScoreAsync(Guid verificationScheduleId, CreateVerificationChecklistSubmissionDto dto, CancellationToken ct = default);

        // Fetches consolidated result overview for organizer final selection decision
        Task<VerificationConsolidatedResultDto?> GetConsolidatedResultAsync(Guid verificationScheduleId, CancellationToken ct = default);
        Task<ApplicationVerificationSummaryDto?> GetApplicationConsolidatedSummaryAsync(Guid applicationId, CancellationToken ct = default);
    }
}