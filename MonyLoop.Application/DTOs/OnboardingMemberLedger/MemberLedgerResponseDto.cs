using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.DTOs.OnboardingMemberLedger
{
    public class MemberLedgerResponseDto
    {

        public Guid MemberLedgerId { get; set; }
        public Guid UserId { get; set; }
        public Guid OnboardingCaseId { get; set; }
        public Guid ActivatedByAdminId { get; set; }
        public DateTime ActivatedAt { get; set; }

        // Display information for Organizer/Admin payment selection
        public string MemberName { get; set; } = string.Empty;
        public string CircleTitle { get; set; } = string.Empty;
        public int? SlotNumber { get; set; }
    }
}
