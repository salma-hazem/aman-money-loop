using Microsoft.Extensions.Options;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Infrastructure.Services.Email.Models;
using System.Net;
using System.Net.Mail;

namespace MonyLoop.Infrastructure.Services.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IEmailTemplateRenderer _renderer;
        private readonly IOptions<SmtpOptions> _smtpOptions;

        public SmtpEmailSender(
            IEmailTemplateRenderer renderer,
            IOptions<SmtpOptions> smtpOptions)
        {
            _renderer = renderer;
            _smtpOptions = smtpOptions;
        }

        public async Task SendWelcomeEmailAsync(
            string toEmail,
            string userName,
            string temporaryPassword,
            string loginUrl,
            CancellationToken ct = default)
        {
            var model = new WelcomeEmailModel
            {
                UserName = userName,
                To = toEmail,
                TemporaryPassword = temporaryPassword,
                LoginUrl = loginUrl
            };

            var html = await _renderer.RenderAsync(
                "WelcomeEmail",
                model);

            await SendEmailAsync(
                toEmail,
                "Welcome to MonyLoop",
                html,
                ct);
        }

        public async Task SendOtpEmailAsync(
            string toEmail,
            string userName,
            string code,
            int expiryMinutes,
            CancellationToken ct = default)
        {
            var model = new OtpEmailModel
            {
                UserName = userName,
                Code = code,
                ExpiryMinutes = expiryMinutes
            };

            var html = await _renderer.RenderAsync(
                "OtpEmail",
                model);

            await SendEmailAsync(
                toEmail,
                "Your Verification Code",
                html,
                ct);
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlBody,
            CancellationToken ct = default)
        {
            var smtp = _smtpOptions.Value;

            using var client =
                new SmtpClient(
                    smtp.Host,
                    smtp.Port)
                {
                    Credentials =
                        new NetworkCredential(
                            smtp.Username,
                            smtp.Password),

                    EnableSsl = smtp.EnableSsl
                };

            using var message =
                new MailMessage
                {
                    From =
                        new MailAddress(
                            smtp.FromEmail,
                            smtp.FromName),

                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

            message.To.Add(toEmail);

            await client.SendMailAsync(
                message,
                ct);
        }
    }
}
