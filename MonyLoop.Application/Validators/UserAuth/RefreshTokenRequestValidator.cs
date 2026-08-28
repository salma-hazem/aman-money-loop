using FluentValidation;
using MonyLoop.Application.DTOs.UserAuth;

namespace MonyLoop.Application.Validators.UserAuth;

public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequestDto>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
