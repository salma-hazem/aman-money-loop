using MonyLoop.Domain.Constants.Onboarding___Member_Ledger;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.Domain.Entities.Onboarding___Member_Ledger
{
    public class Document
    {
        public Guid DocumentId { get; set; }
        public Guid OnboardingCaseId { get; set; }
        public Guid DocumentRequirementId { get; set; }
        public Guid? ReviewedByUserId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
        public string? RejectionReason { get; set; }
        public DateTime UploadedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        //relations
        public OnboardingCase? OnboardingCase { get; set; }
        public DocumentRequirement? DocumentRequirement { get; set; }
        public ApplicationUser? ReviewedByUser { get; set; }
    }
}
