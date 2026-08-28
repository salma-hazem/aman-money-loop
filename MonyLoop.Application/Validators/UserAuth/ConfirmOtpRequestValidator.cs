using FluentValidation;
using MonyLoop.Application.DTOs.UserAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.Validators.UserAuth
{
    public class ConfirmOtpRequestValidator : AbstractValidator<ConfirmOtpRequestDto>
    {
        public ConfirmOtpRequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().Length(6).Matches(@"^\d+$")
                .WithMessage("OTP code must be exactly 6 digits.");
        }
    }
}
