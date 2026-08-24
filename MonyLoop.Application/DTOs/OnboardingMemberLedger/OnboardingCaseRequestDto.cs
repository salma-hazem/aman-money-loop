using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.DTOs.OnboardingMemberLedger
{
    public class OnboardingCaseRequestDto
    {
        public Guid UserId { get; set; }
        public Guid MembershipAgreementId { get; set; }
        public Guid OrganizerId { get; set; }

    }
}
