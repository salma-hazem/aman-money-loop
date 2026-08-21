
using MonyLoop.Application.DTOs.AgreementPayment.PaymentTransaction;

namespace MonyLoop.Application.ServicesAbstractions.AgreementPayment
{
    public interface IPaymentTransactionService
    {
        Task<PaymentsOverviewResponse> GetPaymentsByMemberLedgerAsync(
            Guid memberLedgerId,Guid requesterId,bool isAdmin);

        Task<PaymentTransactionResponse> RecordPayInAsync(
            RecordPayInRequest request,
            Guid recordedByUserId, bool isAdmin);

        Task<PaymentTransactionResponse> RecordPayOutAsync(
            RecordPayOutRequest request,
            Guid recordedByUserId, bool isAdmin);
        Task<PaymentReceiptResponse?> GetReceiptAsync(
            Guid transactionId,Guid requesterId,
    bool isAdmin);
    }
}