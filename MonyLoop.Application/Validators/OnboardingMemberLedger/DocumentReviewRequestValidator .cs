using FluentValidation;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Domain.Constants.Onboarding___Member_Ledger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.Validators.OnboardingMemberLedger
{
    public class DocumentReviewRequestValidator : AbstractValidator<DocumentReviewRequestDto>
    {
        private static readonly string[] AllowedStatuses =
            Enum.GetNames(typeof(DocumentStatus));

        public DocumentReviewRequestValidator()
        {
            RuleFor(x => x.DocumentId).NotEmpty();

            RuleFor(x => x.NewStatus)
                .NotEmpty()
                .Must(status => AllowedStatuses.Contains(status))
                .WithMessage($"NewStatus must be one of: {string.Join(", ", AllowedStatuses)}");

            RuleFor(x => x.RejectionReason)
                .NotEmpty()
                .When(x => x.NewStatus == DocumentStatus.Rejected.ToString())
                .WithMessage("Rejection reason is required when rejecting a document.");
        }
    }
}
