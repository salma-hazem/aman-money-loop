using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs.AgreementPayment.PaymentTransaction;
using MonyLoop.Application.ServicesAbstractions.AgreementPayment;
using Microsoft.AspNetCore.Authorization;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.API.Controllers.AgreementPayment
{
    [ApiController]
    [Route("api/payment-transactions")]
    public class PaymentTransactionsController : ControllerBase
    {
        private readonly IPaymentTransactionService _paymentTransactionService;
        private readonly IPaymentReceiptPdfService _paymentReceiptPdfService;

        public PaymentTransactionsController(
            IPaymentTransactionService paymentTransactionService,
            IPaymentReceiptPdfService paymentReceiptPdfService)
        {
            _paymentTransactionService = paymentTransactionService;
            _paymentReceiptPdfService = paymentReceiptPdfService;
        }

        [Authorize]
        [HttpGet("member-ledger/{memberLedgerId:guid}")]
        public async Task<IActionResult> GetPaymentsByMemberLedger(
            Guid memberLedgerId)
        {
            try
            {
                var userIdClaim = User.FindFirst("uid")?.Value;

                if (!Guid.TryParse(userIdClaim, out var requesterId))
                {
                    return Unauthorized();
                }

                var isAdmin = User.IsInRole(ApplicationRole.Admin);
                var payments =
                    await _paymentTransactionService
                        .GetPaymentsByMemberLedgerAsync(memberLedgerId, requesterId, isAdmin);

                return Ok(payments);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize(Roles = $"{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpPost("pay-ins")]
        public async Task<IActionResult> RecordPayIn(
    [FromBody] RecordPayInRequest request)
        {
            try
            {
                var userIdClaim =
                    User.FindFirst("uid")?.Value;

                if (!Guid.TryParse(
                    userIdClaim,
                    out var recordedByUserId))
                {
                    return Unauthorized(new
                    {
                        message =
                            "Authenticated user ID could not be determined."
                    });
                }

                var isAdmin =
                    User.IsInRole(ApplicationRole.Admin);

                var transaction =
                    await _paymentTransactionService.RecordPayInAsync(
                        request,
                        recordedByUserId,
                        isAdmin);

                return CreatedAtAction(
                    nameof(GetReceipt),
                    new
                    {
                        transactionId =
                            transaction.PaymentTransactionId
                    },
                    transaction);
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

        [Authorize(Roles = $"{ApplicationRole.Admin},{ApplicationRole.Organizer}")]
        [HttpPost("pay-outs")]
        public async Task<IActionResult> RecordPayOut(
    [FromBody] RecordPayOutRequest request)
        {
            try
            {
                var userIdClaim =
                    User.FindFirst("uid")?.Value;

                if (!Guid.TryParse(
                    userIdClaim,
                    out var recordedByUserId))
                {
                    return Unauthorized(new
                    {
                        message =
                            "Authenticated user ID could not be determined."
                    });
                }

                var isAdmin =
                    User.IsInRole(ApplicationRole.Admin);

                var transaction =
                    await _paymentTransactionService.RecordPayOutAsync(
                        request,
                        recordedByUserId,
                        isAdmin);

                return CreatedAtAction(
                    nameof(GetReceipt),
                    new
                    {
                        transactionId =
                            transaction.PaymentTransactionId
                    },
                    transaction);
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

        [Authorize]
        [HttpGet("{transactionId:guid}/receipt")]
        public async Task<IActionResult> GetReceipt(Guid transactionId)
        {
            try
            {
                var userIdClaim = User.FindFirst("uid")?.Value;

                if (!Guid.TryParse(
                    userIdClaim,
                    out var requesterId))
                {
                    return Unauthorized();
                }

                var isAdmin =
                    User.IsInRole(ApplicationRole.Admin);
                var receipt =
                    await _paymentTransactionService.GetReceiptAsync(transactionId, requesterId, isAdmin);

                if (receipt is null)
                {
                    return NotFound(new
                    {
                        message = "Payment transaction was not found."
                    });
                }

                var pdfBytes =
                    _paymentReceiptPdfService.GenerateReceiptPdf(receipt);

                var fileName =
                    $"Receipt-{receipt.ReceiptNumber ?? transactionId.ToString()}.pdf";

                return File(
                    pdfBytes,
                    "application/pdf",
                    fileName);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}