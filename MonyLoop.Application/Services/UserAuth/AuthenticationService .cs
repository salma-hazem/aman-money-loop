using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.UserAuth;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Domain.Constants.UserAuth;
using MonyLoop.Domain.Entities.UserAuth;
using MonyLoop.Domain.Interfaces;
using System.Security.Claims;

namespace MonyLoop.Application.Services.UserAuth
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOTPService _otpService;
        private readonly IJwtService _jwtService;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IOTPService otpService,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _otpService = otpService;
            _jwtService = jwtService;
        }

        public async Task<Result<Guid>> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default)
        {
            var email = request.Email.Trim();
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return Result<Guid>.Fail(Error.Validation("Auth.EmailExists", "This email address is already registered."));

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                UserName = email,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                NationalId = request.NationalId.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                MustChangePassword = false,
                EmailConfirmed = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var identityResult = await _userManager.CreateAsync(user, request.Password);
            if (!identityResult.Succeeded)
                return Result<Guid>.Fail(identityResult.ToValidationErrors());

            var roleResult = await _userManager.AddToRoleAsync(user, ApplicationRole.Member);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return Result<Guid>.Fail(roleResult.ToValidationErrors());
            }

            var otpResult = await _otpService.GenerateAndSendAsync(
                user.Id,
                user.Email!,
                user.FirstName,
                OTPPurpose.RegistrationConfirmation,
                ct);

            if (otpResult.IsFailure)
                return Result<Guid>.Fail(otpResult.Errors.ToList());

            return Result<Guid>.Ok(user.Id);
        }

        public async Task<Result> ConfirmRegistrationOtpAsync(ConfirmOtpRequestDto request, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return Result.Fail(Error.NotFound("Auth.UserNotFound", "User was not found."));

            if (user.EmailConfirmed)
                return Result.Ok();

            var verifyResult = await _otpService.VerifyAsync(
                request.UserId,
                request.Code,
                OTPPurpose.RegistrationConfirmation,
                ct);

            if (verifyResult.IsFailure)
                return verifyResult;

            user.EmailConfirmed = true;
            user.UpdatedAt = DateTime.UtcNow;
            var updateResult = await _userManager.UpdateAsync(user);

            return updateResult.Succeeded
                ? Result.Ok()
                : Result.Fail(updateResult.ToValidationErrors());
        }

        public async Task<Result> ResendRegistrationOtpAsync(ResendRegistrationOtpRequestDto request, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return Result.Fail(Error.NotFound("Auth.UserNotFound", "User was not found."));

            if (user.EmailConfirmed)
                return Result.Fail(Error.Validation("Auth.EmailAlreadyConfirmed", "The email address is already confirmed."));

            return await _otpService.GenerateAndSendAsync(
                user.Id,
                user.Email!,
                user.FirstName,
                OTPPurpose.RegistrationConfirmation,
                ct);
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(request.Email.Trim());
            if (user == null)
                return InvalidCredentials();

            if (!user.IsActive)
                return Result<AuthResponseDto>.Fail(Error.Forbidden("Auth.AccountDisabled", "This account is disabled."));

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
                return InvalidCredentials();

            if (!user.EmailConfirmed)
                return Result<AuthResponseDto>.Fail(Error.Validation("Auth.EmailNotConfirmed", "Confirm your email before logging in."));

            var roles = await _userManager.GetRolesAsync(user);
            var response = await GenerateAuthResponseAsync(user, roles, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<AuthResponseDto>.Ok(response);
        }

        public async Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken ct = default)
        {
            ClaimsPrincipal? principal;
            try
            {
                principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
            }
            catch (SecurityTokenException)
            {
                return Result<AuthResponseDto>.Fail(Error.InvalidCredentials("Auth.InvalidToken", "The access token is invalid."));
            }
            catch (ArgumentException)
            {
                return Result<AuthResponseDto>.Fail(Error.InvalidCredentials("Auth.InvalidToken", "The access token is invalid."));
            }

            if (principal == null || !TryGetUserId(principal, out var userId))
                return Result<AuthResponseDto>.Fail(Error.InvalidCredentials("Auth.InvalidToken", "The access token is invalid."));

            var storedToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken, ct);
            if (storedToken == null || storedToken.UserId != userId || storedToken.IsRevoked)
                return Result<AuthResponseDto>.Fail(Error.InvalidCredentials("Auth.InvalidRefreshToken", "The refresh token is invalid."));

            if (storedToken.ExpiresAt <= DateTime.UtcNow)
                return Result<AuthResponseDto>.Fail(Error.InvalidCredentials("Auth.ExpiredRefreshToken", "The refresh token has expired."));

            var user = storedToken.User ?? await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || !user.IsActive)
                return Result<AuthResponseDto>.Fail(Error.InvalidCredentials("Auth.InvalidRefreshToken", "The refresh token is invalid."));

            storedToken.IsRevoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;

            var roles = await _userManager.GetRolesAsync(user);
            var response = await GenerateAuthResponseAsync(user, roles, ct);
            storedToken.ReplacedByToken = response.RefreshToken;
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<AuthResponseDto>.Ok(response);
        }

        public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Result.Fail(Error.NotFound("Auth.UserNotFound", "User was not found."));

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
                return Result.Fail(result.ToValidationErrors());

            user.MustChangePassword = false;
            user.UpdatedAt = DateTime.UtcNow;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return Result.Fail(updateResult.ToValidationErrors());

            await _unitOfWork.RefreshTokens.RevokeAllActiveAsync(user.Id, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(request.Email.Trim());
            if (user == null || !user.IsActive || !user.EmailConfirmed)
                return Result.Ok();

            await _otpService.GenerateAndSendAsync(
                user.Id,
                user.Email!,
                user.FirstName,
                OTPPurpose.PasswordReset,
                ct);

            return Result.Ok();
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(request.Email.Trim());
            if (user == null || !user.IsActive)
                return Result.Fail(Error.Validation("Auth.InvalidPasswordReset", "The password reset request is invalid."));

            var otpResult = await _otpService.VerifyAsync(
                user.Id,
                request.Code,
                OTPPurpose.PasswordReset,
                ct);

            if (otpResult.IsFailure)
                return otpResult;

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);
            if (!resetResult.Succeeded)
                return Result.Fail(resetResult.ToValidationErrors());

            user.MustChangePassword = false;
            user.UpdatedAt = DateTime.UtcNow;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return Result.Fail(updateResult.ToValidationErrors());

            await _unitOfWork.RefreshTokens.RevokeAllActiveAsync(user.Id, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Ok();
        }

        private async Task<AuthResponseDto> GenerateAuthResponseAsync(ApplicationUser user, IList<string> roles, CancellationToken ct)
        {
            var refreshTokenValue = _jwtService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.Add(_jwtService.RefreshTokenLifetime),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken, ct);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = $"{user.FirstName} {user.LastName}",
                AccessToken = _jwtService.GenerateAccessToken(user, roles),
                RefreshToken = refreshTokenValue,
                MustChangePassword = user.MustChangePassword,
                Roles = roles
            };
        }

        private static Result<AuthResponseDto> InvalidCredentials() =>
            Result<AuthResponseDto>.Fail(Error.InvalidCredentials(
                "Auth.InvalidCredentials",
                "The email or password is incorrect."));

        private static bool TryGetUserId(ClaimsPrincipal principal, out Guid userId)
        {
            var standardId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(standardId, out userId))
                return true;

            return Guid.TryParse(principal.FindFirstValue("uid"), out userId);
        }

    }
}
