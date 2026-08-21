using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs.AgreementPayment.MembershipAgreement;
using MonyLoop.Application.ServicesAbstractions.AgreementPayment;
using Microsoft.AspNetCore.Authorization;
using MonyLoop.Domain.Entities.UserAuth;

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
        [Authorize(Roles = ApplicationRole.Organizer)]
        [HttpPost]
        public async Task<IActionResult> CreateAgreement(
            [FromBody] CreateMembershipAgreementRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("uid")?.Value;

                if (!Guid.TryParse(userIdClaim, out var organizerId))
                {
                    return Unauthorized(new
                    {
                        message = "Authenticated organizer ID could not be determined."
                    });
                }
                var agreement =
                    await _membershipAgreementService.CreateAgreementAsync(request,
        organizerId);

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
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        //member view agreement 
        [Authorize(Roles = $"{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetAgreementById(Guid id)
        {
            try
            {
                var userIdClaim =User.FindFirst("uid")?.Value;
                if (!Guid.TryParse(userIdClaim,out var requesterId))
                {
                    return Unauthorized(new
                    {
                        message = "Authenticated user ID could not be determined."
                    });
                }

                var isAdmin =User.IsInRole(ApplicationRole.Admin);
                var agreement =await _membershipAgreementService
                        .GetAgreementByIdAsync(
                            id,
                            requesterId,
                            isAdmin);

                if (agreement is null)
                {
                    return NotFound(new
                    {
                        message = "Membership agreement was not found."
                    });
                }

                return Ok(agreement);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        //member accept agreement 
        [HttpPost("{id:guid}/accept")]
        public async Task<IActionResult> AcceptAgreement( Guid id, [FromQuery] string token)
        {
            try
            {
                var agreement =
                    await _membershipAgreementService.AcceptAgreementAsync( id, token);

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
        public async Task<IActionResult> DeclineAgreement(  Guid id,  [FromQuery] string token)
        {
            try
            {
                var agreement =
                    await _membershipAgreementService.DeclineAgreementAsync(id, token);

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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }


        //member view agreement for response with token
        [HttpGet("{id:guid}/response")]
        public async Task<IActionResult> GetAgreementForResponse( Guid id, [FromQuery] string token)
        {
            try
            {
                var agreement =
                    await _membershipAgreementService
                        .GetAgreementForResponseAsync(
                            id,
                            token);

                if (agreement is null)
                {
                    return NotFound(new
                    {
                        message = "Membership agreement was not found."
                    });
                }

                return Ok(agreement);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
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
 


