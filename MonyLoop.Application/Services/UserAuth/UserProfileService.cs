using Microsoft.AspNetCore.Identity;
using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.UserAuth;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Domain.Constants.UserAuth;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.Application.Services.UserAuth;

public class UserProfileService : IUserProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOTPService _otpService;

    public UserProfileService(
        UserManager<ApplicationUser> userManager,
        IOTPService otpService)
    {
        _userManager = userManager;
        _otpService = otpService;
    }

    public async Task<Result<UserProfileResponseDto>> GetAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user is null
            ? Result<UserProfileResponseDto>.Fail(UserNotFound())
            : Result<UserProfileResponseDto>.Ok(Map(user));
    }

    public async Task<Result<UserProfileResponseDto>> UpdateAsync(
        Guid userId,
        UpdateProfileRequestDto request,
        CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result<UserProfileResponseDto>.Fail(UserNotFound());
        }

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.PhoneNumber = request.PhoneNumber.Trim();
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded
            ? Result<UserProfileResponseDto>.Ok(Map(user))
            : Result<UserProfileResponseDto>.Fail(result.ToValidationErrors());
    }

    public async Task<Result> RequestEmailChangeAsync(
        Guid userId,
        RequestEmailChangeDto request,
        CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Fail(UserNotFound());
        }

        var newEmail = request.NewEmail.Trim();
        if (string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Fail(Error.Validation(
                "Profile.EmailUnchanged",
                "The new email must be different from the current email."));
        }

        if (await _userManager.FindByEmailAsync(newEmail) is not null)
        {
            return Result.Fail(Error.Validation(
                "Profile.EmailExists",
                "This email address is already registered."));
        }

        var otpResult = await _otpService.GenerateAndSendAsync(
            user.Id,
            newEmail,
            user.FirstName,
            OTPPurpose.EmailChange,
            ct);

        if (otpResult.IsFailure)
        {
            return otpResult;
        }

        user.PendingEmail = newEmail;
        user.UpdatedAt = DateTime.UtcNow;
        var updateResult = await _userManager.UpdateAsync(user);
        return updateResult.Succeeded
            ? Result.Ok()
            : Result.Fail(updateResult.ToValidationErrors());
    }

    public async Task<Result<UserProfileResponseDto>> ConfirmEmailChangeAsync(
        Guid userId,
        ConfirmEmailChangeDto request,
        CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result<UserProfileResponseDto>.Fail(UserNotFound());
        }

        if (string.IsNullOrWhiteSpace(user.PendingEmail))
        {
            return Result<UserProfileResponseDto>.Fail(Error.Validation(
                "Profile.NoPendingEmailChange",
                "No pending email change was found."));
        }

        var verifyResult = await _otpService.VerifyAsync(
            user.Id,
            request.Code,
            OTPPurpose.EmailChange,
            ct);

        if (verifyResult.IsFailure)
        {
            return Result<UserProfileResponseDto>.Fail(verifyResult.Errors.ToList());
        }

        var newEmail = user.PendingEmail;
        if (await _userManager.FindByEmailAsync(newEmail) is not null)
        {
            return Result<UserProfileResponseDto>.Fail(Error.Validation(
                "Profile.EmailExists",
                "This email address is already registered."));
        }

        user.Email = newEmail;
        user.UserName = newEmail;
        user.NormalizedEmail = _userManager.NormalizeEmail(newEmail);
        user.NormalizedUserName = _userManager.NormalizeName(newEmail);
        user.EmailConfirmed = true;
        user.PendingEmail = null;
        user.UpdatedAt = DateTime.UtcNow;

        var updateResult = await _userManager.UpdateAsync(user);
        return updateResult.Succeeded
            ? Result<UserProfileResponseDto>.Ok(Map(user))
            : Result<UserProfileResponseDto>.Fail(updateResult.ToValidationErrors());
    }

    private static UserProfileResponseDto Map(ApplicationUser user) => new()
    {
        UserId = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email!,
        PhoneNumber = user.PhoneNumber,
        NationalId = user.NationalId
    };

    private static Error UserNotFound() =>
        Error.NotFound("Profile.UserNotFound", "User was not found.");
}
