using FluentValidation;
using MonyLoop.Application.DTOs.UserAuth;

namespace MonyLoop.Application.Validators.UserAuth;

public sealed class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequestDto>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(128);
        RuleFor(x => x.Code).NotEmpty().Length(6).Matches(@"^\d+$");
        RuleFor(x => x.NewPassword).ApplyPasswordRules();
        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword)
            .WithMessage("Password confirmation does not match.");
    }
}
