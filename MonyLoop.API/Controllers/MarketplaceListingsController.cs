using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs.CircleRequestManagement;
using MonyLoop.Application.ServicesAbstractions;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.API.Controllers
{
    // FR10-FR11: Marketplace browsing is public — guests and members can view without logging in.
    public class MarketplaceListingsController : ApiBaseController
    {
        private readonly IMarketplaceListingService _service;
        public MarketplaceListingsController(IMarketplaceListingService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MarketplaceListingSummaryDto>>> GetActive(
            [FromQuery] MarketplaceListingQueryDto query)
        {
            var result = await _service.GetActiveListingsAsync(query);
            return HandleResult(result);
        }
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MarketplaceListingDetailDto>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return HandleResult(result);
        }

        [Authorize(Roles = $"{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult<MarketplaceListingDetailDto>> UpdateStatus(
            Guid id,
            [FromBody] UpdateMarketplaceListingStatusDto dto)
        {
            var result = await _service.UpdateStatusAsync(id, dto.Status);
            return HandleResult(result);
        }
    }
}