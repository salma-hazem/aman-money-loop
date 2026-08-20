using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs.CircleRequestManagement;
using MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;
using MonyLoop.Domain.Constants;

namespace MonyLoop.API.Controllers.CircleRequestManagement;

[Authorize(Roles = SystemRoles.CircleOrganizer)]
[Route("api/circle-requests")]
public sealed class CircleRequestsController : ApiBaseController
{
    private readonly ICircleRequestService _service;

    public CircleRequestsController(ICircleRequestService service)
    {
        _service = service;
    }

    [HttpPost("new")]
    public async Task<ActionResult<CircleRequestResponseDto>> CreateNew(
        [FromBody] CreateNewCircleRequestDto dto,
        CancellationToken cancellationToken)
    {
        if (!TryGetOrganizerId(out var organizerId))
        {
            return Unauthorized();
        }

        var result = await _service.CreateNewAsync(organizerId, dto, cancellationToken);
        if (result.IsFailure)
        {
            return HandleResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.RequestId }, result.Value);
    }

    [HttpPost("replacement")]
    public async Task<ActionResult<CircleRequestResponseDto>> CreateReplacement(
        [FromBody] CreateReplacementCircleRequestDto dto,
        CancellationToken cancellationToken)
    {
        if (!TryGetOrganizerId(out var organizerId))
        {
            return Unauthorized();
        }

        var result = await _service.CreateReplacementAsync(organizerId, dto, cancellationToken);
        if (result.IsFailure)
        {
            return HandleResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.RequestId }, result.Value);
    }

    [HttpPut("{id:guid}/new")]
    public async Task<ActionResult<CircleRequestResponseDto>> UpdateNew(
        Guid id,
        [FromBody] UpdateNewCircleRequestDto dto,
        CancellationToken cancellationToken)
    {
        if (!TryGetOrganizerId(out var organizerId))
        {
            return Unauthorized();
        }

        return HandleResult(await _service.UpdateNewAsync(organizerId, id, dto, cancellationToken));
    }

    [HttpPut("{id:guid}/replacement")]
    public async Task<ActionResult<CircleRequestResponseDto>> UpdateReplacement(
        Guid id,
        [FromBody] UpdateReplacementCircleRequestDto dto,
        CancellationToken cancellationToken)
    {
        if (!TryGetOrganizerId(out var organizerId))
        {
            return Unauthorized();
        }

        return HandleResult(await _service.UpdateReplacementAsync(organizerId, id, dto, cancellationToken));
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<ActionResult<CircleRequestResponseDto>> Submit(Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetOrganizerId(out var organizerId))
        {
            return Unauthorized();
        }

        return HandleResult(await _service.SubmitAsync(organizerId, id, cancellationToken));
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<ActionResult<CircleRequestResponseDto>> Publish(Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetOrganizerId(out var organizerId))
        {
            return Unauthorized();
        }

        return HandleResult(await _service.PublishAsync(organizerId, id, cancellationToken));
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<CircleRequestResponseDto>> Cancel(Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetOrganizerId(out var organizerId))
        {
            return Unauthorized();
        }

        return HandleResult(await _service.CancelAsync(organizerId, id, cancellationToken));
    }

    [HttpGet("mine")]
    public async Task<ActionResult<IReadOnlyList<CircleRequestSummaryDto>>> GetMine(CancellationToken cancellationToken)
    {
        if (!TryGetOrganizerId(out var organizerId))
        {
            return Unauthorized();
        }

        return HandleResult(await _service.GetMineAsync(organizerId, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CircleRequestResponseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetOrganizerId(out var organizerId))
        {
            return Unauthorized();
        }

        return HandleResult(await _service.GetByIdAsync(organizerId, id, cancellationToken));
    }

    private bool TryGetOrganizerId(out Guid organizerId) =>
        CurrentUserIdResolver.TryGet(User, out organizerId);
}
