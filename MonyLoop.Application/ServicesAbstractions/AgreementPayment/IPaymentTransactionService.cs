
using MonyLoop.Application.DTOs.AgreementPayment.PaymentTransaction;

namespace MonyLoop.Application.ServicesAbstractions.AgreementPayment
{
    public interface IPaymentTransactionService
    {
        Task<PaymentsOverviewResponse> GetPaymentsByMemberLedgerAsync(
            Guid memberLedgerId);

        Task<PaymentTransactionResponse> RecordPayInAsync(
            RecordPayInRequest request);

        Task<PaymentTransactionResponse> RecordPayOutAsync(
           RecordPayOutRequest request);
        Task<PaymentReceiptResponse?> GetReceiptAsync(
            Guid transactionId);
    }
}