using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.DTOs.OnboardingMemberLedger
{
    public class DocumentRequirementResponseDto
    {
        public Guid DocumentRequirementId { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsRequired { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }
}
