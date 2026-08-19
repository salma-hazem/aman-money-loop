using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MonyLoop.Application.ServicesAbstractions;
using MonyLoop.Application.Settings;

namespace MonyLoop.Infrastructure
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(
            IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendAgreementEmailAsync(
            string recipientEmail,
            string memberName,
            Guid agreementId,
            string responseToken)
        {
            var responseUrl =
                $"{_settings.FrontendBaseUrl}/agreement-response" +
                $"?agreementId={agreementId}" +
                $"&token={Uri.EscapeDataString(responseToken)}";

            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    _settings.SenderName,
                    _settings.SenderEmail));

            message.To.Add(
                MailboxAddress.Parse(recipientEmail));

            message.Subject = "Aman Money Loop - Membership Agreement";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"""
                <html>
                <body>
                    <p>Dear {System.Net.WebUtility.HtmlEncode(memberName)},</p>

                    <p>
                        Your Aman Money Loop membership agreement
                        is ready for review.
                    </p>

                    <p>
                        Please use the link below to view the agreement
                        and accept or decline it:
                    </p>

                    <p>
                        <a href="{responseUrl}">
                            View Membership Agreement
                        </a>
                    </p>

                    <p>
                        If the agreement has expired,
                        the response page will no longer allow
                        acceptance or decline.
                    </p>

                    <p>
                        Aman Money Loop
                    </p>
                </body>
                </html>
                """
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            await client.ConnectAsync(
                _settings.SmtpServer,
                _settings.Port,
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _settings.Username,
                _settings.Password);

            await client.SendAsync(message);

            await client.DisconnectAsync(true);
        }
    }
}