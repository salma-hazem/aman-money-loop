using Hangfire;
using MonyLoop.Application.Common;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Domain.Constants.UserAuth;
using MonyLoop.Domain.Entities.UserAuth;
using MonyLoop.Domain.Interfaces;
using System.Security.Cryptography;

namespace MonyLoop.Application.Services.UserAuth;

public class OTPService : IOTPService
{
    private const int ExpiryMinutes = 10;
    private const int MaxAttempts = 5;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _emailSender;
    private readonly IRateLimiterService _rateLimiter;

    public OTPService(
        IUnitOfWork unitOfWork,
        IEmailSender emailSender,
        IRateLimiterService rateLimiter)
    {
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
        _rateLimiter = rateLimiter;
    }

    public async Task<Result> GenerateAndSendAsync(
        Guid userId,
        string email,
        string userName,
        OTPPurpose purpose,
        CancellationToken ct = default)
    {
        var cooldownKey = $"otp-request:{userId}:{purpose}";
        if (!await _rateLimiter.IsAllowedAsync(cooldownKey, TimeSpan.FromSeconds(60)))
        {
            return Result.Fail(Error.Validation(
                "OTP.RateLimited",
                "Please wait before requesting another OTP."));
        }

        await _unitOfWork.OTPTokens.InvalidateExistingTokensAsync(userId, purpose, ct);

        var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        var otp = new OTPToken
        {
            OTPTokenId = Guid.NewGuid(),
            UserId = userId,
            Code = code,
            Purpose = purpose,
            ExpiresAt = DateTime.UtcNow.AddMinutes(ExpiryMinutes),
            IsUsed = false,
            AttemptsCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.OTPTokens.AddAsync(otp, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        BackgroundJob.Enqueue(() => _emailSender.SendOtpEmailAsync(
            email,
            userName,
            code,
            ExpiryMinutes,
            CancellationToken.None));

        return Result.Ok();
    }

    public async Task<Result> VerifyAsync(
        Guid userId,
        string code,
        OTPPurpose purpose,
        CancellationToken ct = default)
    {
        var otp = await _unitOfWork.OTPTokens.GetLatestActiveAsync(userId, purpose, ct);
        if (otp is null)
        {
            return Result.Fail(Error.NotFound(
                "OTP.NotFound",
                "No active OTP was found. Please request a new code."));
        }

        if (otp.IsUsed)
        {
            return Result.Fail(Error.Validation(
                "OTP.AlreadyUsed",
                "This OTP has already been used."));
        }

        if (otp.ExpiresAt <= DateTime.UtcNow)
        {
            return Result.Fail(Error.Validation(
                "OTP.Expired",
                "The OTP has expired. Please request a new code."));
        }

        if (otp.AttemptsCount >= MaxAttempts)
        {
            return Result.Fail(Error.Validation(
                "OTP.MaxAttemptsExceeded",
                "The maximum number of attempts has been exceeded."));
        }

        if (otp.Code != code)
        {
            otp.AttemptsCount++;
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Fail(Error.Validation(
                "OTP.Invalid",
                "The OTP is invalid."));
        }

        otp.IsUsed = true;
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
