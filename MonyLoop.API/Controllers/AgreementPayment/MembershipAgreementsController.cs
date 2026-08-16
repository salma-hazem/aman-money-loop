using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs.AgreementPayment.MembershipAgreement;
using MonyLoop.Application.ServicesAbstractions.AgreementPayment;

namespace MonyLoop.API.Controllers.AgreementPayment
{
    [ApiController]
    [Route("api/membership-agreements")]
    public class MembershipAgreementsController : ControllerBase
    {
        private readonly IMembershipAgreementService _membershipAgreementService;

        public MembershipAgreementsController(
            IMembershipAgreementService membershipAgreementService)
        {
            _membershipAgreementService = membershipAgreementService;
        }

        //organizer send agreement 
        [HttpPost]
        public async Task<IActionResult> CreateAgreement(
            [FromBody] CreateMembershipAgreementRequest request)
        {
            try
            {
                var agreement =
                    await _membershipAgreementService.CreateAgreementAsync(request);

                return CreatedAtAction(
                    nameof(GetAgreementById),
                    new { id = agreement.MembershipAgreementId },
                    agreement);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        //member view agreement 
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetAgreementById(Guid id)
        {
            var agreement =
                await _membershipAgreementService.GetAgreementByIdAsync(id);

            if (agreement is null)
                return NotFound();

            return Ok(agreement);
        }

        //member accept agreement 
        [HttpPost("{id:guid}/accept")]
        public async Task<IActionResult> AcceptAgreement(Guid id)
        {
            try
            {
                var agreement =
                    await _membershipAgreementService.AcceptAgreementAsync(id);

                if (agreement is null)
                    return NotFound();

                return Ok(agreement);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        //member decline agreement 
        [HttpPost("{id:guid}/decline")]
        public async Task<IActionResult> DeclineAgreement(Guid id)
        {
            try
            {
                var agreement =
                    await _membershipAgreementService.DeclineAgreementAsync(id);

                if (agreement is null)
                    return NotFound();

                return Ok(agreement);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
 


