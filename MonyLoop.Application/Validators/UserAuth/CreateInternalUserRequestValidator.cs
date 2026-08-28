using FluentValidation;
using MonyLoop.Application.DTOs.UserAuth;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.Application.Validators.UserAuth;

public sealed class CreateInternalUserRequestValidator : AbstractValidator<CreateInternalUserRequestDto>
{
    public CreateInternalUserRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(128);
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^01[0125][0-9]{8}$")
            .WithMessage("Invalid Egyptian phone number format.");
        RuleFor(x => x.NationalId)
            .Length(14)
            .Matches(@"^\d+$")
            .When(x => !string.IsNullOrWhiteSpace(x.NationalId))
            .WithMessage("National ID must be exactly 14 digits.");
        RuleFor(x => x.Role)
            .Must(role => role is ApplicationRole.Admin or ApplicationRole.Organizer)
            .WithMessage("Role must be Admin or Organizer.");
    }
}
