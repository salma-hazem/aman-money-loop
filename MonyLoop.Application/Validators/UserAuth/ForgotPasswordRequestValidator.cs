using FluentValidation;
using MonyLoop.Application.DTOs.UserAuth;

namespace MonyLoop.Application.Validators.UserAuth;

public sealed class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequestDto>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(128);
    }
}
