using FluentValidation;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.Validators.OnboardingMemberLedger
{
    public class OnboardingCaseRequestValidator : AbstractValidator<OnboardingCaseRequestDto>
    {
        public OnboardingCaseRequestValidator()
        {
            RuleFor(x => x.MembershipAgreementId).NotEmpty();
            RuleFor(x => x.OrganizerId).NotEmpty();
        }
    }
}
