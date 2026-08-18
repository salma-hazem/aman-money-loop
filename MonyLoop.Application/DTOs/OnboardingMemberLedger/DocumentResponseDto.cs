using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.DTOs.OnboardingMemberLedger
{
    public class DocumentResponseDto
    {
        public Guid DocumentId { get; set; }
        public Guid OnboardingCaseId { get; set; }
        public Guid DocumentRequirementId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid? ReviewedByUserId { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? RejectionReason { get; set; }
    }
}
