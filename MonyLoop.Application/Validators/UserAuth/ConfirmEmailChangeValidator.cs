using FluentValidation;
using MonyLoop.Application.DTOs.UserAuth;

namespace MonyLoop.Application.Validators.UserAuth;

public sealed class ConfirmEmailChangeValidator : AbstractValidator<ConfirmEmailChangeDto>
{
    public ConfirmEmailChangeValidator()
    {
        RuleFor(x => x.Code).NotEmpty().Length(6).Matches(@"^\d+$");
    }
}
