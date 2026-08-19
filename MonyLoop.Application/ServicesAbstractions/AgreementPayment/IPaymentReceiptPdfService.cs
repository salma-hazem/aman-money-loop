using MonyLoop.Application.DTOs.AgreementPayment.PaymentTransaction;

namespace MonyLoop.Application.ServicesAbstractions.AgreementPayment
{
    public interface IPaymentReceiptPdfService
    {
        byte[] GenerateReceiptPdf(PaymentReceiptResponse receipt);
    }
}