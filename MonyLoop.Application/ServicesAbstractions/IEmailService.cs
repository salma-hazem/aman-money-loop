namespace MonyLoop.Application.ServicesAbstractions
{
    public interface IEmailService
    {
        Task SendAgreementEmailAsync(
            string recipientEmail,
            string memberName,
            Guid agreementId,
            string responseToken);
    }
}