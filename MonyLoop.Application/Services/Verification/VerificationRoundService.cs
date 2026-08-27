using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MonyLoop.Application.DTOs.Verification;
using MonyLoop.Application.ServicesAbstractions.Verification;
using MonyLoop.Domain.Entities.Verification;
using MonyLoop.Domain.Interfaces;
using MonyLoop.Domain.Interfaces.Verification;

namespace MonyLoop.Application.Services.Verification
{
    public class VerificationRoundService : IVerificationRoundService
    {
        private readonly IVerificationRoundRepository _roundRepository;
        private readonly IVerificationCriterionRepository _criterionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VerificationRoundService(
            IVerificationRoundRepository roundRepository,
            IVerificationCriterionRepository criterionRepository,
            IUnitOfWork unitOfWork)
        {
            _roundRepository = roundRepository;
            _criterionRepository = criterionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<VerificationRoundResponseDto> CreateRoundAsync(CreateVerificationRoundDto dto, CancellationToken ct = default)
        {
            var round = new VerificationRound
            {
                VerificationRoundId = Guid.NewGuid(),
                CircleId = dto.CircleId,
                ReviewedByUserId = dto.ReviewedByUserId,
                RoundName = dto.RoundName,
                Format = dto.Format
            };

            await _roundRepository.AddAsync(round, ct);

            foreach (var c in dto.Criteria)
            {
                var criterion = new VerificationCriterion
                {
                    VerificationCriterionId = Guid.NewGuid(),
                    VerificationRoundId = round.VerificationRoundId,
                    CriterionName = c.CriterionName,
                    Weight = c.Weight,
                    DisplayOrder = c.DisplayOrder,
                    IsActive = c.IsActive
                };
                await _criterionRepository.AddAsync(criterion, ct);
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return await GetRoundByIdAsync(round.VerificationRoundId, ct)
                ?? throw new InvalidOperationException("Failed to create verification round.");
        }

        public async Task<VerificationRoundResponseDto?> GetRoundByIdAsync(Guid verificationRoundId, CancellationToken ct = default)
        {
            var round = await _roundRepository.GetVerificationRoundByIdAsync(verificationRoundId, ct);
            if (round == null) return null;

            var criteria = await _criterionRepository.GetByVerificationRoundIdAsync(verificationRoundId, ct);

            return MapToRoundDto(round, criteria);
        }

        public async Task<IReadOnlyList<VerificationRoundResponseDto>> GetRoundsByCircleIdAsync(Guid circleId, CancellationToken ct = default)
        {
            var rounds = await _roundRepository.GetRoundsByCircleIdAsync(circleId, ct);
            var result = new List<VerificationRoundResponseDto>();

            foreach (var r in rounds)
            {
                var criteria = await _criterionRepository.GetByVerificationRoundIdAsync(r.VerificationRoundId, ct);
                result.Add(MapToRoundDto(r, criteria));
            }

            return result;
        }

        public async Task<VerificationRoundResponseDto?> UpdateRoundAsync(Guid verificationRoundId, UpdateVerificationRoundDto dto, CancellationToken ct = default)
        {
            var existingRound = await _roundRepository.GetVerificationRoundByIdAsync(verificationRoundId, ct);
            if (existingRound == null) return null;

            existingRound.RoundName = dto.RoundName;
            existingRound.Format = dto.Format;
            if (dto.ReviewedByUserId != null)
            {
                existingRound.ReviewedByUserId = dto.ReviewedByUserId;
            }

            existingRound.Criteria = dto.Criteria.Select(c => new VerificationCriterion
            {
                VerificationCriterionId = c.VerificationCriterionId ?? Guid.NewGuid(),
                VerificationRoundId = verificationRoundId,
                CriterionName = c.CriterionName,
                Weight = c.Weight,
                DisplayOrder = c.DisplayOrder,
                IsActive = c.IsActive
            }).ToList();

            await _roundRepository.UpdateByIdAsync(verificationRoundId, existingRound, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return await GetRoundByIdAsync(verificationRoundId, ct);
        }

        private static VerificationRoundResponseDto MapToRoundDto(VerificationRound round, IEnumerable<VerificationCriterion> criteria)
        {
            return new VerificationRoundResponseDto
            {
                VerificationRoundId = round.VerificationRoundId,
                CircleId = round.CircleId,
                ReviewedByUserId = round.ReviewedByUserId,
                RoundName = round.RoundName,
                Format = round.Format,
                Criteria = criteria.Select(c => new VerificationCriterionResponseDto
                {
                    VerificationCriterionId = c.VerificationCriterionId,
                    VerificationRoundId = c.VerificationRoundId,
                    CriterionName = c.CriterionName,
                    Weight = c.Weight,
                    DisplayOrder = c.DisplayOrder,
                    IsActive = c.IsActive
                }).ToList()
            };
        }
    }
}