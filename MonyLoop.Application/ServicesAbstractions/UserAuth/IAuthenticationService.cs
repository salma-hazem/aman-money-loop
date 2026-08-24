using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.UserAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.ServicesAbstractions.UserAuth
{
    public interface IAuthenticationService
    {
        Task<Result<Guid>> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default);
        Task<Result> ConfirmRegistrationOtpAsync(ConfirmOtpRequestDto request, CancellationToken ct = default);
        Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken ct = default);
        Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken ct = default);
        Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken ct = default);

        Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken ct = default);
        Task<Result> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken ct = default);

    }
}
