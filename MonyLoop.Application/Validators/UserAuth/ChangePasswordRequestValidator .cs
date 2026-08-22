using FluentValidation;
using MonyLoop.Application.DTOs.UserAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.Validators.UserAuth
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequestDto>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty();
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8)
                .Must(p => p.Any(char.IsDigit)).WithMessage("Password must contain at least one digit.");
        }
    }

}
