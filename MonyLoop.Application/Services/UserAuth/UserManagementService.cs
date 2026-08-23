using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.UserAuth;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Domain.Entities.UserAuth;
using System.Security.Cryptography;

namespace MonyLoop.Application.Services.UserAuth;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _configuration = configuration;
    }

    public async Task<Result<UserResponseDto>> CreateInternalUserAsync(
        Guid adminId,
        CreateInternalUserRequestDto request,
        CancellationToken ct = default)
    {
        var email = request.Email.Trim();
        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            return Result<UserResponseDto>.Fail(Error.Validation(
                "Users.EmailExists",
                "This email address is already registered."));
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            NationalId = NormalizeOptional(request.NationalId),
            RegisteredByAdminId = adminId,
            EmailConfirmed = true,
            MustChangePassword = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var temporaryPassword = GenerateTemporaryPassword();
        var createResult = await _userManager.CreateAsync(user, temporaryPassword);
        if (!createResult.Succeeded)
        {
            return Result<UserResponseDto>.Fail(createResult.ToValidationErrors());
        }

        var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return Result<UserResponseDto>.Fail(roleResult.ToValidationErrors());
        }

        var frontendBaseUrl = _configuration["ApplicationUrls:FrontendBaseUrl"] ?? "http://localhost:4200";
        await _emailSender.SendWelcomeEmailAsync(
            email,
            $"{user.FirstName} {user.LastName}",
            temporaryPassword,
            $"{frontendBaseUrl.TrimEnd('/')}/login",
            ct);

        return Result<UserResponseDto>.Ok(new UserResponseDto
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            NationalId = user.NationalId,
            EmailConfirmed = user.EmailConfirmed,
            MustChangePassword = user.MustChangePassword,
            Roles = (await _userManager.GetRolesAsync(user)).ToList()
        });
    }

    private static string GenerateTemporaryPassword()
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnopqrstuvwxyz";
        const string digits = "23456789";
        const string special = "!@#$%&*";
        const string all = upper + lower + digits + special;

        var characters = new char[12];
        characters[0] = Pick(upper);
        characters[1] = Pick(lower);
        characters[2] = Pick(digits);
        characters[3] = Pick(special);

        for (var i = 4; i < characters.Length; i++)
        {
            characters[i] = Pick(all);
        }

        for (var i = characters.Length - 1; i > 0; i--)
        {
            var swapIndex = RandomNumberGenerator.GetInt32(i + 1);
            (characters[i], characters[swapIndex]) = (characters[swapIndex], characters[i]);
        }

        return new string(characters);
    }

    private static char Pick(string characters) =>
        characters[RandomNumberGenerator.GetInt32(characters.Length)];

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
