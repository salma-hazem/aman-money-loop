using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.API.Authentication;
using MonyLoop.Application.DTOs.CircleRequestManagement;
using MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.API.Controllers.CircleRequestManagement;

[Authorize(Roles = ApplicationRole.Admin + "," + ApplicationRole.Organizer)]
[Route("api/circles")]
public sealed class CirclesController : ApiBaseController
{
    private readonly ICircleRegistryService _service;

    public CirclesController(ICircleRegistryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CircleResponseDto>>> GetAll(
        [FromQuery] CircleStatus? status,
        CancellationToken cancellationToken) =>
        HandleResult(await _service.GetAllAsync(status, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CircleResponseDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        HandleResult(await _service.GetByIdAsync(id, cancellationToken));

    [HttpGet("{id:guid}/slots")]
    public async Task<ActionResult<IReadOnlyList<CircleSlotResponseDto>>> GetSlots(Guid id, CancellationToken cancellationToken) =>
        HandleResult(await _service.GetSlotsAsync(id, cancellationToken));

    [Authorize(Roles = ApplicationRole.Admin)]
    [HttpPost("{id:guid}/slots/{slotNumber:int}/vacate")]
    public async Task<ActionResult<CircleSlotResponseDto>> Vacate(
        Guid id,
        int slotNumber,
        CancellationToken cancellationToken)
    {
        if (!CurrentUserIdResolver.TryGet(User, out var adminId))
        {
            return Unauthorized();
        }

        return HandleResult(await _service.VacateSlotAsync(adminId, id, slotNumber, cancellationToken));
    }
}
