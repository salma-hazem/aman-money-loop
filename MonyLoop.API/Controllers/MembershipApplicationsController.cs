using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs;
using MonyLoop.Application.ServicesAbstractions;

namespace MonyLoop.API.Controllers
{
    public class MembershipApplicationsController : ApiBaseController
    {
        private readonly IMembershipApplicationService _service;

        public MembershipApplicationsController(IMembershipApplicationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<MembershipApplicationDetailDto>> Create(
            [FromBody] CreateMembershipApplicationDto dto)
        {
            var result = await _service.CreateApplicationAsync(dto);
            return HandleResult(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MembershipApplicationDetailDto>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return HandleResult(result);
        }

        [HttpGet("by-listing/{listingId:guid}")]
        public async Task<ActionResult<IReadOnlyList<MembershipApplicationSummaryDto>>> GetByListing(
            Guid listingId)
        {
            var result = await _service.GetByListingIdAsync(listingId);
            return HandleResult(result);
        }

        [HttpPost("{id:guid}/shortlist")]
        public async Task<ActionResult<MembershipApplicationDetailDto>> Shortlist(Guid id)
        {
            var result = await _service.ShortlistAsync(id);
            return HandleResult(result);
        }

        [HttpPost("{id:guid}/reject")]
        public async Task<ActionResult<MembershipApplicationDetailDto>> Reject(Guid id)
        {
            var result = await _service.RejectAsync(id);
            return HandleResult(result);
        }
    }
}