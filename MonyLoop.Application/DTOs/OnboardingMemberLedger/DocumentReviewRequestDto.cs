using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.DTOs.OnboardingMemberLedger
{
    public class DocumentReviewRequestDto
    {
        public Guid DocumentId { get; set; }
        public string NewStatus { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
    }
}
