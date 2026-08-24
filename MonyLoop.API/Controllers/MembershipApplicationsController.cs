using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.Common;
using MonyLoop.Application.DTOs;
using MonyLoop.Application.ServicesAbstractions;
using MonyLoop.Domain.Entities.UserAuth;
namespace MonyLoop.API.Controllers
{
    public class MembershipApplicationsController : ApiBaseController
    {
        private readonly IMembershipApplicationService _service;
        public MembershipApplicationsController(IMembershipApplicationService service)
        {
            _service = service;
        }
        // FR10: Marketplace is public — guests and registered members can apply, no login required.
        [HttpPost]
        public async Task<ActionResult<MembershipApplicationDetailDto>> Create(
            [FromBody] CreateMembershipApplicationDto dto)
        {
            var result = await _service.CreateApplicationAsync(dto);
            return HandleResult(result);
        }
        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MembershipApplicationDetailDto>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return HandleResult(result);
        }
        // FR12: Organizer Dashboard — applicant pipeline is Organizer/Admin only.
        [Authorize(Roles = $"{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpGet("by-listing/{listingId:guid}")]
        public async Task<ActionResult<PagedResult<MembershipApplicationSummaryDto>>> GetByListing(
            Guid listingId, [FromQuery] PaginationRequestDto pagination)
        {
            var result = await _service.GetByListingIdAsync(listingId, pagination);
            return HandleResult(result);
        }
        // FR14: Organizers manage pipeline stages.
        [Authorize(Roles = $"{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpPost("{id:guid}/shortlist")]
        public async Task<ActionResult<MembershipApplicationDetailDto>> Shortlist(Guid id)
        {
            var result = await _service.ShortlistAsync(id);
            return HandleResult(result);
        }
        [Authorize(Roles = $"{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpPost("{id:guid}/reject")]
        public async Task<ActionResult<MembershipApplicationDetailDto>> Reject(Guid id)
        {
            var result = await _service.RejectAsync(id);
            return HandleResult(result);
        }
    }
}
