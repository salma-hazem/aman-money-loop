using Mony_Loop.Domain.Constants.Onboarding___Member_Ledger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Domain.Entities.Onboarding___Member_Ledger
{
    public class Document
    {
        //        Document
        //Attribute   Type
        //Id  Guid
        //OnboardingCaseId    Guid(FK → OnboardingCase)
        //ReviewedByUserId Guid? (FK → User)
        //DocumentRequirementId Guid(FK → DocumentRequirement)
        //FileName string
        //FilePath    string
        //FileSize    long
        //UploadedAt  DateTime
        //Status  DocumentStatus(enum)
        //ReviewedAt DateTime?
        //RejectionReason string?
        public Guid DocumentId { get; set; }
        public Guid OnboardingCaseId { get; set; }
        public Guid DocumentRequirementId { get; set; }
        public Guid? ReviewedByUserId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string Status { get; set; } = DocumentStatus.Pending;
        public string? RejectionReason { get; set; }

        public DateTime UploadedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }

        // Navigation Properties

        public OnboardingCase? OnboardingCase { get; set; }

        public DocumentRequirement? DocumentRequirement { get; set; }

        // public User? ReviewedByUser { get; set; }

    }
}
