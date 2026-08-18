using Microsoft.AspNetCore.Mvc;
using MonyLoop.Application.DTOs.AgreementPayment.PaymentTransaction;
using MonyLoop.Application.ServicesAbstractions.AgreementPayment;


namespace MonyLoop.API.Controllers.AgreementPayment
{
    [ApiController]
    [Route("api/payment-transactions")]
    public class PaymentTransactionsController : ControllerBase
    {
        private readonly IPaymentTransactionService _paymentTransactionService;

        public PaymentTransactionsController(
            IPaymentTransactionService paymentTransactionService)
        {
            _paymentTransactionService = paymentTransactionService;
        }

        [HttpGet("member-ledger/{memberLedgerId:guid}")]
        public async Task<IActionResult> GetPaymentsByMemberLedger(
            Guid memberLedgerId)
        {
            var payments =
                await _paymentTransactionService
                    .GetPaymentsByMemberLedgerAsync(memberLedgerId);

            return Ok(payments);
        }

        [HttpPost("pay-ins")]
        public async Task<IActionResult> RecordPayIn(
        [FromBody] RecordPayInRequest request)
            {
                var transaction =
                    await _paymentTransactionService.RecordPayInAsync(request);

                return CreatedAtAction(
                    nameof(GetReceipt),
                    new { transactionId = transaction.PaymentTransactionId },
                    transaction);
            }

        [HttpPost("pay-outs")]
        public async Task<IActionResult> RecordPayOut(
    [FromBody] RecordPayOutRequest request)
        {
            try
            {
                var transaction =
                    await _paymentTransactionService.RecordPayOutAsync(request);

                return CreatedAtAction(
                    nameof(GetReceipt),
                    new { transactionId = transaction.PaymentTransactionId },
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
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("{transactionId:guid}/receipt")]
        public async Task<IActionResult> GetReceipt(Guid transactionId)
        {
            try
            {
                var receipt =
                    await _paymentTransactionService.GetReceiptAsync(transactionId);

                if (receipt is null)
                    return NotFound();

                return Ok(receipt);
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