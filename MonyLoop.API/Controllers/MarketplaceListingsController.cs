using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs.CircleRequestManagement;
using MonyLoop.Application.ServicesAbstractions;

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
        public async Task<ActionResult<IReadOnlyList<MarketplaceListingSummaryDto>>> GetActive()
        {
            var result = await _service.GetActiveListingsAsync();
            return HandleResult(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MarketplaceListingDetailDto>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return HandleResult(result);
        }
    }
}