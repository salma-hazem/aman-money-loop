using MonyLoop.Application.Common;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Domain.Constants.UserAuth;
using MonyLoop.Domain.Entities.UserAuth;
using MonyLoop.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using Hangfire;

namespace MonyLoop.Application.Services.UserAuth
{
    public class OTPService : IOTPService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private const int ExpiryMinutes = 10;
        private const int MaxAttempts = 5;


        public OTPService(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        public async Task<Result> GenerateAndSendAsync(Guid userId, string email, string userName, OTPPurpose purpose, CancellationToken ct)
        {
            await _unitOfWork.OTPTokens.InvalidateExistingTokensAsync(userId, purpose, ct);

            var code = new Random().Next(100000, 999999).ToString();
            var otp = new OTPToken()
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

            BackgroundJob.Enqueue(() => _emailSender.SendOtpEmailAsync(email, userName, code, ExpiryMinutes, CancellationToken.None));
            return Result<string>.Ok(code);

        }

        public async Task<Result> VerifyAsync(Guid userId, string code, OTPPurpose purpose, CancellationToken ct = default)
        {
            var otp = await _unitOfWork.OTPTokens.GetLatestActiveAsync(userId, purpose, ct);

            if (otp == null)
                return Result.Fail(Error.NotFound("OTP.NotFound", "لا يوجد كود تحقق فعال، اطلب كود جديد."));

            if (otp.IsUsed)
                return Result.Fail(Error.Validation("OTP.AlreadyUsed", "هذا الكود مستخدم بالفعل."));

            if (otp.ExpiresAt < DateTime.UtcNow)
                return Result.Fail(Error.Validation("OTP.Expired", "انتهت صلاحية الكود، اطلب كود جديد."));

            if (otp.AttemptsCount >= MaxAttempts)
                return Result.Fail(Error.Validation("OTP.MaxAttemptsExceeded", "تم تجاوز عدد المحاولات المسموح، اطلب كود جديد."));

            if (otp.Code != code)
            {
                otp.AttemptsCount++;
                await _unitOfWork.SaveChangesAsync(ct);
                return Result.Fail(Error.Validation("OTP.Invalid", "الكود الذي أدخلته غير صحيح."));
            }

            otp.IsUsed = true;
            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Ok();

        }


    }
}
