using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.API.Authentication;
using MonyLoop.Application.DTOs.UserAuth;
using MonyLoop.Application.ServicesAbstractions.UserAuth;

namespace MonyLoop.API.Controllers.UserAuth;

[Authorize]
[Route("api/profile")]
public sealed class ProfileController : ApiBaseController
{
    private readonly IUserProfileService _profileService;

    public ProfileController(IUserProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<ActionResult<UserProfileResponseDto>> Get(CancellationToken ct)
    {
        if (!CurrentUserIdResolver.TryGet(User, out var userId))
        {
            return Unauthorized();
        }

        return HandleResult(await _profileService.GetAsync(userId, ct));
    }

    [HttpPut]
    public async Task<ActionResult<UserProfileResponseDto>> Update(
        [FromBody] UpdateProfileRequestDto request,
        CancellationToken ct)
    {
        if (!CurrentUserIdResolver.TryGet(User, out var userId))
        {
            return Unauthorized();
        }

        return HandleResult(await _profileService.UpdateAsync(userId, request, ct));
    }

    [HttpPost("email-change/request")]
    public async Task<IActionResult> RequestEmailChange(
        [FromBody] RequestEmailChangeDto request,
        CancellationToken ct)
    {
        if (!CurrentUserIdResolver.TryGet(User, out var userId))
        {
            return Unauthorized();
        }

        return HandleResult(await _profileService.RequestEmailChangeAsync(userId, request, ct));
    }

    [HttpPost("email-change/confirm")]
    public async Task<ActionResult<UserProfileResponseDto>> ConfirmEmailChange(
        [FromBody] ConfirmEmailChangeDto request,
        CancellationToken ct)
    {
        if (!CurrentUserIdResolver.TryGet(User, out var userId))
        {
            return Unauthorized();
        }

        return HandleResult(await _profileService.ConfirmEmailChangeAsync(userId, request, ct));
    }
}
