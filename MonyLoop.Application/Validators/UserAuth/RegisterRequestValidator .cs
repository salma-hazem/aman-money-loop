using FluentValidation;
using MonyLoop.Application.DTOs.UserAuth;

namespace MonyLoop.Application.Validators.UserAuth;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(128);
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^01[0125][0-9]{8}$")
            .WithMessage("Invalid Egyptian phone number format.");
        RuleFor(x => x.NationalId)
            .NotEmpty()
            .WithMessage("National ID is required.")
            .Length(14)
            .Matches(@"^\d+$")
            .WithMessage("National ID must be exactly 14 digits.");
        RuleFor(x => x.Password).ApplyPasswordRules();
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Password confirmation does not match.");
    }
}
