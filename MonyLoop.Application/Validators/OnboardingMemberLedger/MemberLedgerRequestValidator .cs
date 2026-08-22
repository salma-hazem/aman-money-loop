using FluentValidation;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.Validators.OnboardingMemberLedger
{
    public class MemberLedgerRequestValidator : AbstractValidator<MemberLedgerRequestDto>
    {
        public MemberLedgerRequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.OnboardingCaseId).NotEmpty();
        }
    }
}
