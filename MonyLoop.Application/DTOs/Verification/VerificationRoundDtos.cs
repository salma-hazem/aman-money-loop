using System;
using System.Collections.Generic;
using MonyLoop.Domain.Constants.Verification;

namespace MonyLoop.Application.DTOs.Verification
{
    public class CreateVerificationRoundDto
    {
        public Guid CircleId { get; set; }
        public Guid ReviewedByUserId { get; set; }
        public string RoundName { get; set; } = string.Empty;
        public VerificationFormat Format { get; set; } = VerificationFormat.Video;
        public List<CreateVerificationCriterionDto> Criteria { get; set; } = new();
    }
    public class VerificationRoundResponseDto
    {
        public Guid VerificationRoundId { get; set; }
        public Guid CircleId { get; set; }
        public Guid ReviewedByUserId { get; set; }
        public string RoundName { get; set; } = string.Empty;
        public VerificationFormat Format { get; set; }
        public List<VerificationCriterionResponseDto> Criteria { get; set; } = new();
    }

    public class CreateVerificationCriterionDto
    {
        public string CriterionName { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class VerificationCriterionResponseDto
    {
        public Guid VerificationCriterionId { get; set; }
        public Guid VerificationRoundId { get; set; }
        public string CriterionName { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
    public class UpdateVerificationRoundDto
    {
        public string RoundName { get; set; } = string.Empty;
        public VerificationFormat Format { get; set; }
        public List<UpdateVerificationCriterionDto> Criteria { get; set; } = new();
    }

    public class UpdateVerificationCriterionDto
    {
        public Guid? VerificationCriterionId { get; set; } // Nullable: populated for existing rows, null for new ones
        public string CriterionName { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}