using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.UserAuth;

namespace MonyLoop.Application.ServicesAbstractions.UserAuth;

public interface IUserManagementService
{
    Task<Result<UserResponseDto>> CreateInternalUserAsync(
        Guid adminId,
        CreateInternalUserRequestDto request,
        CancellationToken ct = default);
}
