using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs.UserAuth;
using MonyLoop.Application.ServicesAbstractions.UserAuth;

namespace MonyLoop.API.Controllers.UserAuth
{
    public class AuthController : ApiBaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Guid>> Register([FromBody] RegisterRequestDto request, CancellationToken ct)
        {
            var result = await _authenticationService.RegisterAsync(request, ct);
            return HandleResult(result);
        }

        [HttpPost("confirm-otp")]
        public async Task<IActionResult> ConfirmOtp([FromBody] ConfirmOtpRequestDto request, CancellationToken ct)
        {
            var result = await _authenticationService.ConfirmRegistrationOtpAsync(request, ct);
            return HandleResult(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request, CancellationToken ct)
        {
            var result = await _authenticationService.LoginAsync(request, ct);
            return HandleResult(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken ct)
        {
            var result = await _authenticationService.RefreshTokenAsync(request, ct);
            return HandleResult(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirst("uid")!.Value);
            var result = await _authenticationService.ChangePasswordAsync(userId, request, ct);
            return HandleResult(result);
        }
    }
}
