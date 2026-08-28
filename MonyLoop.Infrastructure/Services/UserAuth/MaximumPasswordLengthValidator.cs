using Microsoft.AspNetCore.Identity;

namespace MonyLoop.Infrastructure.Services.UserAuth;

public sealed class MaximumPasswordLengthValidator<TUser> : IPasswordValidator<TUser>
    where TUser : class
{
    private const int MaximumLength = 64;

    public Task<IdentityResult> ValidateAsync(
        UserManager<TUser> manager,
        TUser user,
        string? password)
    {
        if (password is not null && password.Length > MaximumLength)
        {
            return Task.FromResult(IdentityResult.Failed(new IdentityError
            {
                Code = "PasswordTooLong",
                Description = $"Passwords must be at most {MaximumLength} characters."
            }));
        }

        return Task.FromResult(IdentityResult.Success);
    }
}
