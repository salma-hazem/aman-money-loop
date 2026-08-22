using FluentValidation;
using MonyLoop.Application.DTOs.UserAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.Validators.UserAuth
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.NationalId).NotEmpty().Length(14).Matches(@"^\d+$")
                .WithMessage("National ID must be exactly 14 digits.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^01[0125][0-9]{8}$")
                .WithMessage("Invalid Egyptian phone number format.");
        }
    }
}
