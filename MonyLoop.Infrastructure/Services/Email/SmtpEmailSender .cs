using Microsoft.Extensions.Configuration;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Infrastructure.Services.Email.Models;
using System.Net;
using System.Net.Mail;

namespace MonyLoop.Infrastructure.Services.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IEmailTemplateRenderer _renderer;
        private readonly IConfiguration _configuration;

        public SmtpEmailSender(
            IEmailTemplateRenderer renderer,
            IConfiguration configuration)
        {
            _renderer = renderer;
            _configuration = configuration;
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
            var smtpHost =
                _configuration["Smtp:Host"];

            var smtpPort =
                int.Parse(
                    _configuration["Smtp:Port"]
                    ?? "587");

            var smtpUser =
                _configuration["Smtp:Username"];

            var smtpPass =
                _configuration["Smtp:Password"];

            var fromEmail =
                _configuration["Smtp:FromEmail"];

            using var client =
                new SmtpClient(
                    smtpHost,
                    smtpPort)
                {
                    Credentials =
                        new NetworkCredential(
                            smtpUser,
                            smtpPass),

                    EnableSsl = true
                };

            using var message =
                new MailMessage
                {
                    From =
                        new MailAddress(
                            fromEmail!,
                            "MonyLoop"),

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