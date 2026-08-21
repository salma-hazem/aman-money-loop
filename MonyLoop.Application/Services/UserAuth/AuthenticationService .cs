using Microsoft.AspNetCore.Identity;
using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.UserAuth;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Domain.Constants.UserAuth;
using MonyLoop.Domain.Entities.UserAuth;
using MonyLoop.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.Services.UserAuth
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOTPService _otpService;
        private readonly IEmailSender _emailSender;
        private readonly IJwtService _jwtService;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IOTPService otpService,
            IEmailSender emailSender,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _otpService = otpService;
            _emailSender = emailSender;
            _jwtService = jwtService;
        }

        public async Task<Result<Guid>> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return Result<Guid>.Fail(Error.Validation("Auth.EmailExists", "هذا البريد الإلكتروني مستخدم بالفعل."));

            var temporaryPassword = GenerateTemporaryPassword();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                NationalId = request.NationalId,
                PhoneNumber = request.PhoneNumber,
                MustChangePassword = true,
                EmailConfirmed = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var identityResult = await _userManager.CreateAsync(user, temporaryPassword);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors
                    .Select(e => Error.Validation(e.Code, e.Description))
                    .ToList();
                return Result<Guid>.Fail(errors);
            }

            await _userManager.AddToRoleAsync(user, ApplicationRole.Member);

            await _emailSender.SendWelcomeEmailAsync(user.Email, $"{user.FirstName} {user.LastName}", temporaryPassword, "https://monyloop.com/login", ct);

            await _otpService.GenerateAndSendAsync(user.Id, user.Email, user.FirstName, OTPPurpose.RegistrationConfirmation, ct);

            return Result<Guid>.Ok(user.Id);
        }

        public async Task<Result> ConfirmRegistrationOtpAsync(ConfirmOtpRequestDto request, CancellationToken ct = default)
        {
            var verifyResult = await _otpService.VerifyAsync(request.UserId, request.Code, OTPPurpose.RegistrationConfirmation, ct);
            if (verifyResult.IsFailure)
                return verifyResult;

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return Result.Fail(Error.NotFound("Auth.UserNotFound", "المستخدم غير موجود."));

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return Result.Ok();
        }

        private static string GenerateTemporaryPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$";
            var bytes = new byte[10];
            RandomNumberGenerator.Fill(bytes);
            return new string(bytes.Select(b => chars[b % chars.Length]).ToArray());
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Result<AuthResponseDto>.Fail(Error.InvalidCredentials("Auth.InvalidCredentials", "البريد الإلكتروني أو كلمة المرور غير صحيحة."));

            if (!user.IsActive)
                return Result<AuthResponseDto>.Fail(Error.Forbidden("Auth.AccountDisabled", "هذا الحساب معطل."));

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
                return Result<AuthResponseDto>.Fail(Error.InvalidCredentials("Auth.InvalidCredentials", "البريد الإلكتروني أو كلمة المرور غير صحيحة."));

            if (!user.EmailConfirmed)
                return Result<AuthResponseDto>.Fail(Error.Validation("Auth.EmailNotConfirmed", "يرجى تأكيد بريدك الإلكتروني أولاً."));

            var roles = await _userManager.GetRolesAsync(user);
            var response = await GenerateAuthResponseAsync(user, roles, ct);

            return Result<AuthResponseDto>.Ok(response);
        }

        public async Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken ct = default)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
                return Result<AuthResponseDto>.Fail(Error.InvalidCredentials("Auth.InvalidToken", "التوكن غير صالح."));

            var userIdClaim = principal.FindFirst("uid")?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
                return Result<AuthResponseDto>.Fail(Error.InvalidCredentials("Auth.InvalidToken", "التوكن غير صالح."));

            var storedToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken, ct);
            if (storedToken == null || storedToken.UserId != userId)
                return Result<AuthResponseDto>.Fail(Error.InvalidCredentials("Auth.InvalidRefreshToken", "توكن التحديث غير صالح."));

            if (storedToken.IsRevoked)
                return Result<AuthResponseDto>.Fail(Error.InvalidCredentials("Auth.RevokedToken", "تم إلغاء هذا التوكن."));

            if (storedToken.ExpiresAt < DateTime.UtcNow)
                return Result<AuthResponseDto>.Fail(Error.InvalidCredentials("Auth.ExpiredRefreshToken", "انتهت صلاحية توكن التحديث، يرجى تسجيل الدخول مرة أخرى."));

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Result<AuthResponseDto>.Fail(Error.NotFound("Auth.UserNotFound", "المستخدم غير موجود."));

            // إلغاء التوكن القديم (Rotation)
            storedToken.IsRevoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;

            var roles = await _userManager.GetRolesAsync(user);
            var response = await GenerateAuthResponseAsync(user, roles, ct);

            storedToken.ReplacedByToken = response.RefreshToken;
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<AuthResponseDto>.Ok(response);
        }

        private async Task<AuthResponseDto> GenerateAuthResponseAsync(ApplicationUser user, IList<string> roles, CancellationToken ct)
        {
            var accessToken = _jwtService.GenerateAccessToken(user, roles);
            var refreshTokenValue = _jwtService.GenerateRefreshToken();

            var refreshTokenExpiryDays = 7; // ممكن تجيبها من appsettings لو حابب
            var refreshToken = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiryDays),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = $"{user.FirstName} {user.LastName}",
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                MustChangePassword = user.MustChangePassword,
                Roles = roles
            };
        }

        public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Result.Fail(Error.NotFound("Auth.UserNotFound", "المستخدم غير موجود."));

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
                return Result.Fail(errors);
            }

            user.MustChangePassword = false;
            await _userManager.UpdateAsync(user);

            return Result.Ok();
        }
    }
}
