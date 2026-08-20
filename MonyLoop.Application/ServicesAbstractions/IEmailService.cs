namespace MonyLoop.Application.ServicesAbstractions
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string recipientEmail,
            string subject,
            string htmlBody,
            CancellationToken cancellationToken = default);

        Task SendAgreementEmailAsync(
            string recipientEmail,
            string memberName,
            Guid agreementId,
            string responseToken);
    }
}
