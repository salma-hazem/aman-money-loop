using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.API.Authentication;
using MonyLoop.Application.DTOs.UserAuth;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.API.Controllers.UserAuth;

[Authorize(Roles = ApplicationRole.Admin)]
[Route("api/admin/users")]
public sealed class AdminUsersController : ApiBaseController
{
    private readonly IUserManagementService _userManagementService;

    public AdminUsersController(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create(
        [FromBody] CreateInternalUserRequestDto request,
        CancellationToken ct)
    {
        if (!CurrentUserIdResolver.TryGet(User, out var adminId))
        {
            return Unauthorized();
        }

        var result = await _userManagementService.CreateInternalUserAsync(adminId, request, ct);
        if (result.IsFailure)
        {
            return HandleResult(result);
        }

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }
}
