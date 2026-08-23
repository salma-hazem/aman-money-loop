using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.API.Authentication;
using MonyLoop.Application.DTOs.UserAuth;
using MonyLoop.Application.ServicesAbstractions.UserAuth;

namespace MonyLoop.API.Controllers.UserAuth;

[Route("api/auth")]
public sealed class AuthController : ApiBaseController
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<Guid>> Register(
        [FromBody] RegisterRequestDto request,
        CancellationToken ct)
    {
        var result = await _authenticationService.RegisterAsync(request, ct);
        return result.IsFailure
            ? HandleResult(result)
            : StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [HttpPost("confirm-registration-otp")]
    [HttpPost("confirm-otp")]
    public async Task<IActionResult> ConfirmRegistrationOtp(
        [FromBody] ConfirmOtpRequestDto request,
        CancellationToken ct) =>
        HandleResult(await _authenticationService.ConfirmRegistrationOtpAsync(request, ct));

    [HttpPost("resend-registration-otp")]
    public async Task<IActionResult> ResendRegistrationOtp(
        [FromBody] ResendRegistrationOtpRequestDto request,
        CancellationToken ct) =>
        HandleResult(await _authenticationService.ResendRegistrationOtpAsync(request, ct));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken ct) =>
        HandleResult(await _authenticationService.LoginAsync(request, ct));

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken(
        [FromBody] RefreshTokenRequestDto request,
        CancellationToken ct) =>
        HandleResult(await _authenticationService.RefreshTokenAsync(request, ct));

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequestDto request,
        CancellationToken ct)
    {
        if (!CurrentUserIdResolver.TryGet(User, out var userId))
        {
            return Unauthorized();
        }

        return HandleResult(await _authenticationService.ChangePasswordAsync(userId, request, ct));
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequestDto request,
        CancellationToken ct)
    {
        await _authenticationService.ForgotPasswordAsync(request, ct);
        return Ok(new
        {
            Message = "If the account exists, a password reset code has been sent."
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequestDto request,
        CancellationToken ct) =>
        HandleResult(await _authenticationService.ResetPasswordAsync(request, ct));
}
