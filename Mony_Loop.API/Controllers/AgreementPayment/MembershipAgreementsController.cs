using Microsoft.AspNetCore.Mvc;
using Mony_Loop.Application.ServicesAbstractions.AgreementPayment;

namespace Mony_Loop.API.Controllers.AgreementPayment
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
        public IActionResult CreateAgreement()
        {
            throw new NotImplementedException();
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
        public IActionResult AcceptAgreement(Guid id)
        {
            throw new NotImplementedException();
        }

        //member decline agreement 
        [HttpPost("{id:guid}/decline")]
        public IActionResult DeclineAgreement(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
 


