using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.DTOs.OnboardingMemberLedger
{
    public class DocumentRequestDto
    {
        public Guid OnboardingCaseId { get; set; }
        public Guid DocumentRequirementId { get; set; }
        public string FileName { get; set; } = String.Empty;
        public string FilePath { get; set; } = String.Empty;
        public long FileSize { get; set; }

    }
}
