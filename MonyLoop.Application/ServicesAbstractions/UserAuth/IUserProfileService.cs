using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs.UserAuth;

namespace MonyLoop.Application.ServicesAbstractions.UserAuth;

public interface IUserProfileService
{
    Task<Result<UserProfileResponseDto>> GetAsync(Guid userId, CancellationToken ct = default);
    Task<Result<UserProfileResponseDto>> UpdateAsync(Guid userId, UpdateProfileRequestDto request, CancellationToken ct = default);
    Task<Result> RequestEmailChangeAsync(Guid userId, RequestEmailChangeDto request, CancellationToken ct = default);
    Task<Result<UserProfileResponseDto>> ConfirmEmailChangeAsync(Guid userId, ConfirmEmailChangeDto request, CancellationToken ct = default);
}
