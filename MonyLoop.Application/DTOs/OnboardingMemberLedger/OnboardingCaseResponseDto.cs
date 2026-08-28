using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.DTOs.OnboardingMemberLedger
{
    public class OnboardingCaseResponseDto
    {
        public Guid UserId { get; set; }

        public Guid OnboardingCaseId { get; set; }
        public Guid MembershipAgreementId { get; set; }
        public Guid OrganizerId { get; set; }
        public string FinalStatus { get; set; } = String.Empty;
        public DateTime CreatedAt { get; set; }

        public List<DocumentResponseDto> Documents { get; set; } = [];
    }
}
