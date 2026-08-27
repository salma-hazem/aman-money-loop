using System;
using System.Collections.Generic;

namespace MonyLoop.Application.DTOs.Verification
{
    public class ApplicationVerificationSummaryDto
    {
        public Guid ApplicationId { get; set; }
        public int TotalRoundsCompleted { get; set; }
        public decimal OverallAverageScore { get; set; }
        public List<VerificationConsolidatedResultDto> RoundResults { get; set; } = new();
    }
    public class CreateVerificationChecklistSubmissionDto
    {
        public Guid VerificationScheduleId { get; set; }
        public Guid SubmittedByUserId { get; set; }
        public string? OverallComments { get; set; }
        public List<CreateVerificationCriterionRatingDto> Ratings { get; set; } = new();
    }

    public class VerificationChecklistSubmissionResponseDto
    {
        public Guid VerificationChecklistSubmissionId { get; set; }
        public Guid VerificationScheduleId { get; set; }
        public Guid SubmittedByUserId { get; set; }
        public decimal CompositeScore { get; set; }
        public string? OverallComments { get; set; }
        public DateTime SubmittedAt { get; set; }
        public List<VerificationCriterionRatingResponseDto> CriterionRatings { get; set; } = new();
    }
    public class VerificationConsolidatedResultDto
    {
        public Guid VerificationScheduleId { get; set; }
        public Guid ApplicationId { get; set; }
        public string RoundName { get; set; } = string.Empty;
        public decimal CompositeScore { get; set; }
        public string? OverallComments { get; set; }
        public DateTime SubmittedAt { get; set; }
        public List<VerificationCriterionRatingResponseDto> DetailedRatings { get; set; } = new();
    }
    public class CreateVerificationCriterionRatingDto
    {
        public Guid VerificationCriterionId { get; set; }
        public int Rating { get; set; }
        public string? Comments { get; set; }
    }

    public class VerificationCriterionRatingResponseDto
    {
        public Guid VerificationCriterionRatingId { get; set; }
        public Guid VerificationChecklistSubmissionId { get; set; }
        public Guid VerificationCriterionId { get; set; }
        public int Rating { get; set; }
        public string? Comments { get; set; }
    }
}