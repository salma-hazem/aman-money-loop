using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.API.Authentication;
using MonyLoop.Application.DTOs.CircleRequestManagement;
using MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.API.Controllers.CircleRequestManagement;

[Authorize(Roles = ApplicationRole.Admin)]
[Route("api/admin/circle-requests")]
public sealed class AdminCircleRequestsController : ApiBaseController
{
    private readonly ICircleRequestReviewService _service;

    public AdminCircleRequestsController(ICircleRequestReviewService service)
    {
        _service = service;
    }

    [HttpGet("queue")]
    public async Task<ActionResult<IReadOnlyList<CircleRequestSummaryDto>>> GetQueue(CancellationToken cancellationToken) =>
        HandleResult(await _service.GetQueueAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CircleRequestResponseDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        HandleResult(await _service.GetByIdAsync(id, cancellationToken));

    [HttpPost("{id:guid}/approve")]
    public async Task<ActionResult<CircleRequestResponseDto>> Approve(Guid id, CancellationToken cancellationToken)
    {
        if (!CurrentUserIdResolver.TryGet(User, out var adminId))
        {
            return Unauthorized();
        }

        return HandleResult(await _service.ApproveAsync(adminId, id, cancellationToken));
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<ActionResult<CircleRequestResponseDto>> Reject(
        Guid id,
        [FromBody] DecisionReasonDto dto,
        CancellationToken cancellationToken)
    {
        if (!CurrentUserIdResolver.TryGet(User, out var adminId))
        {
            return Unauthorized();
        }

        return HandleResult(await _service.RejectAsync(adminId, id, dto, cancellationToken));
    }

    [HttpPost("{id:guid}/request-modification")]
    public async Task<ActionResult<CircleRequestResponseDto>> RequestModification(
        Guid id,
        [FromBody] DecisionReasonDto dto,
        CancellationToken cancellationToken)
    {
        if (!CurrentUserIdResolver.TryGet(User, out var adminId))
        {
            return Unauthorized();
        }

        return HandleResult(await _service.RequestModificationAsync(adminId, id, dto, cancellationToken));
    }

    [HttpGet("{id:guid}/audit")]
    public async Task<ActionResult<IReadOnlyList<AuditLogResponseDto>>> GetAudit(Guid id, CancellationToken cancellationToken) =>
        HandleResult(await _service.GetAuditAsync(id, cancellationToken));
}
