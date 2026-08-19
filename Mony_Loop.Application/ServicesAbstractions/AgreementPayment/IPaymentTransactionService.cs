using Mony_Loop.Application.DTOs.AgreementPayment.PaymentTransaction;

namespace Mony_Loop.Application.ServicesAbstractions.AgreementPayment
{
    public interface IPaymentTransactionService
    {
        Task<PaymentsOverviewResponse> GetPaymentsByMemberLedgerAsync(
            Guid memberLedgerId);

        Task<PaymentTransactionResponse> RecordPayInAsync(
           RecordPayInRequest request);

        Task<PaymentReceiptResponse?> GetReceiptAsync(
        Guid transactionId);
    }
}