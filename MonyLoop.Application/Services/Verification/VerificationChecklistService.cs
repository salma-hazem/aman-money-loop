using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MonyLoop.Application.DTOs.Verification;
using MonyLoop.Application.ServicesAbstractions.Verification;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Constants.Verification;
using MonyLoop.Domain.Entities.Verification;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.Verification;

namespace MonyLoop.Application.Services.Verification
{
    public class VerificationChecklistService : IVerificationChecklistService
    {
        private readonly IVerificationChecklistSubmissionRepository _submissionRepository;
        private readonly IVerificationCriterionRatingRepository _ratingRepository;
        private readonly IVerificationScheduleRepository _scheduleRepository;
        private readonly IVerificationRoundRepository _roundRepository;
        private readonly IVerificationCriterionRepository _criterionRepository;
        private readonly IMembershipApplicationRepository _applicationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VerificationChecklistService(
            IVerificationChecklistSubmissionRepository submissionRepository,
            IVerificationCriterionRatingRepository ratingRepository,
            IVerificationScheduleRepository scheduleRepository,
            IVerificationRoundRepository roundRepository,
            IVerificationCriterionRepository criterionRepository,
            IMembershipApplicationRepository applicationRepository,
            IUnitOfWork unitOfWork)
        {
            _submissionRepository = submissionRepository;
            _ratingRepository = ratingRepository;
            _scheduleRepository = scheduleRepository;
            _roundRepository = roundRepository;
            _criterionRepository = criterionRepository;
            _applicationRepository = applicationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<VerificationChecklistSubmissionResponseDto> SubmitChecklistAsync(CreateVerificationChecklistSubmissionDto dto, CancellationToken ct = default)
        {
            // Calculate Weighted Composite Score automatically (1-5 scale)
            decimal compositeScore = await CalculateWeightedCompositeScoreAsync(dto.VerificationScheduleId, dto, ct);

            var submission = new VerificationChecklistSubmission
            {
                VerificationChecklistSubmissionId = Guid.NewGuid(),
                VerificationScheduleId = dto.VerificationScheduleId,
                SubmittedByUserId = dto.SubmittedByUserId,
                CompositeScore = compositeScore,
                OverallComments = dto.OverallComments,
                SubmittedAt = DateTime.UtcNow
            };

            await _submissionRepository.AddAsync(submission, ct);

            foreach (var r in dto.Ratings)
            {
                var rating = new VerificationCriterionRating
                {
                    VerificationCriterionRatingId = Guid.NewGuid(),
                    VerificationChecklistSubmissionId = submission.VerificationChecklistSubmissionId,
                    VerificationCriterionId = r.VerificationCriterionId,
                    Rating = r.Rating,
                    Comments = r.Comments
                };
                await _ratingRepository.AddAsync(rating, ct);
            }
            var schedule = await _scheduleRepository.GetByIdAsync(dto.VerificationScheduleId, ct)
                ?? throw new KeyNotFoundException($"Schedule with ID {dto.VerificationScheduleId} not found.");

            schedule.Status = ScheduleStatus.Completed;
            await _scheduleRepository.UpdateByIdAsync(schedule.VerificationScheduleId, schedule, ct);

            var application = await _applicationRepository.GetByIdAsync(schedule.ApplicationId);
            if (application != null)
            {
                application.Stage = compositeScore >= 3.0m 
                    ? MembershipApplicationStage.VerificationCompleted
                    : MembershipApplicationStage.Rejected;

                await _applicationRepository.UpdateAsync(application);
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return await GetSubmissionByScheduleIdAsync(dto.VerificationScheduleId, ct)
                ?? throw new InvalidOperationException("Failed to save checklist submission.");
        }

        public async Task<VerificationChecklistSubmissionResponseDto?> GetSubmissionByScheduleIdAsync(Guid verificationScheduleId, CancellationToken ct = default)
        {
            var submission = await _submissionRepository.GetByVerificationScheduleIdAsync(verificationScheduleId, ct);
            if (submission == null) return null;

            var ratings = await _ratingRepository.GetBySubmissionIdAsync(submission.VerificationChecklistSubmissionId, ct);

            return MapToSubmissionDto(submission, ratings);
        }

        public async Task<decimal> CalculateWeightedCompositeScoreAsync(Guid verificationScheduleId, CreateVerificationChecklistSubmissionDto dto, CancellationToken ct = default)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(verificationScheduleId, ct)
                ?? throw new KeyNotFoundException($"Schedule with ID {verificationScheduleId} not found.");

            var activeCriteria = await _criterionRepository.GetByVerificationRoundIdAsync(schedule.VerificationRoundId, ct);
            var activeCriteriaList = activeCriteria.Where(c => c.IsActive).ToList();

            if (!activeCriteriaList.Any()) return 0;

            decimal totalWeight = activeCriteriaList.Sum(c => c.Weight);
            if (totalWeight == 0) return 0;

            decimal weightedSum = 0;
            foreach (var ratingDto in dto.Ratings)
            {
                var criterion = activeCriteriaList.FirstOrDefault(c => c.VerificationCriterionId == ratingDto.VerificationCriterionId);
                if (criterion != null)
                {
                    weightedSum += ratingDto.Rating * criterion.Weight;
                }
            }

            return Math.Round(weightedSum / totalWeight, 2);
        }

        public async Task<VerificationConsolidatedResultDto?> GetConsolidatedResultAsync(Guid verificationScheduleId, CancellationToken ct = default)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(verificationScheduleId, ct);
            if (schedule == null) return null;

            var submission = await _submissionRepository.GetByVerificationScheduleIdAsync(verificationScheduleId, ct);
            if (submission == null) return null;

            var round = await _roundRepository.GetVerificationRoundByIdAsync(schedule.VerificationRoundId, ct);
            var ratings = await _ratingRepository.GetBySubmissionIdAsync(submission.VerificationChecklistSubmissionId, ct);

            return new VerificationConsolidatedResultDto
            {
                VerificationScheduleId = schedule.VerificationScheduleId,
                ApplicationId = schedule.ApplicationId,
                RoundName = round?.RoundName ?? string.Empty,
                CompositeScore = submission.CompositeScore,
                OverallComments = submission.OverallComments,
                SubmittedAt = submission.SubmittedAt,
                DetailedRatings = ratings.Select(r => new VerificationCriterionRatingResponseDto
                {
                    VerificationCriterionRatingId = r.VerificationCriterionRatingId,
                    VerificationChecklistSubmissionId = r.VerificationChecklistSubmissionId,
                    VerificationCriterionId = r.VerificationCriterionId,
                    Rating = r.Rating,
                    Comments = r.Comments
                }).ToList()
            };
        }

        private static VerificationChecklistSubmissionResponseDto MapToSubmissionDto(
            VerificationChecklistSubmission submission,
            IEnumerable<VerificationCriterionRating> ratings)
        {
            return new VerificationChecklistSubmissionResponseDto
            {
                VerificationChecklistSubmissionId = submission.VerificationChecklistSubmissionId,
                VerificationScheduleId = submission.VerificationScheduleId,
                SubmittedByUserId = submission.SubmittedByUserId,
                CompositeScore = submission.CompositeScore,
                OverallComments = submission.OverallComments,
                SubmittedAt = submission.SubmittedAt,
                CriterionRatings = ratings.Select(r => new VerificationCriterionRatingResponseDto
                {
                    VerificationCriterionRatingId = r.VerificationCriterionRatingId,
                    VerificationChecklistSubmissionId = r.VerificationChecklistSubmissionId,
                    VerificationCriterionId = r.VerificationCriterionId,
                    Rating = r.Rating,
                    Comments = r.Comments
                }).ToList()
            };
        }
    }
}