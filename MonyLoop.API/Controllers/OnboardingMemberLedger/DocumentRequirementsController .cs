using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs.OnboardingMemberLedger;
using MonyLoop.Application.ServicesAbstractions.OnboardingMemberLedger;

namespace MonyLoop.API.Controllers.OnboardingMemberLedger
{
    [Authorize]
    public class DocumentRequirementsController : ApiBaseController
    {
        private readonly IDocumentRequirementService _documentRequirementService;

        public DocumentRequirementsController(IDocumentRequirementService documentRequirementService)
        {
            _documentRequirementService = documentRequirementService;
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<DocumentRequirementResponseDto>>> GetActiveOrdered(CancellationToken ct)
        {
            var result = await _documentRequirementService.GetActiveOrderedAsync(ct);
            return HandleResult(result);
        }

        [HttpGet("required")]
        public async Task<ActionResult<IEnumerable<DocumentRequirementResponseDto>>> GetRequiredOnly(CancellationToken ct)
        {
            var result = await _documentRequirementService.GetRequiredOnlyAsync(ct);
            return HandleResult(result);
        }
    }
}

