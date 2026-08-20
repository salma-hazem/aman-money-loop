namespace MonyLoop.Application.ServicesAbstractions
{
    public interface IEmailService
    {
        Task SendAgreementEmailAsync(
            string recipientEmail,
            string memberName,
            Guid agreementId,
            string responseToken);

        Task SendMembershipApplicationStatusChangedEmailAsync(
            string recipientEmail,
            string memberName,
            string newStage);
    }
}