using FluentValidation;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.Validators.OnboardingMemberLedger
{
    public class DocumentRequestValidator : AbstractValidator<DocumentRequestDto>
    {
        public DocumentRequestValidator()
        {
            RuleFor(x => x.OnboardingCaseId).NotEmpty();
            RuleFor(x => x.DocumentRequirementId).NotEmpty();
            RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.FilePath).NotEmpty();
            RuleFor(x => x.FileSize).GreaterThan(0).LessThanOrEqualTo(5 * 1024 * 1024)
                .WithMessage("File size must not exceed 5MB.");
        }
    }
}
