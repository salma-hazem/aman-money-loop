using FluentValidation;
using MonyLoop.Application.DTOs.UserAuth;

namespace MonyLoop.Application.Validators.UserAuth;

public sealed class ResendRegistrationOtpRequestValidator : AbstractValidator<ResendRegistrationOtpRequestDto>
{
    public ResendRegistrationOtpRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
