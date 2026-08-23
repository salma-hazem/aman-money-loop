using FluentValidation;
using MonyLoop.Application.DTOs.UserAuth;

namespace MonyLoop.Application.Validators.UserAuth;

public sealed class RequestEmailChangeValidator : AbstractValidator<RequestEmailChangeDto>
{
    public RequestEmailChangeValidator()
    {
        RuleFor(x => x.NewEmail).NotEmpty().EmailAddress().MaximumLength(128);
    }
}
