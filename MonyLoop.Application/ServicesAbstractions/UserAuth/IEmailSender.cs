using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.ServicesAbstractions.UserAuth
{

    public interface IEmailSender
    {
        //generic mail sender
        Task SendEmailAsync(string toEmail,string subject,string htmlBody,CancellationToken ct = default);
        Task SendOtpEmailAsync(string toEmail, string userName, string code, int expiryMinutes, CancellationToken ct = default);
        Task SendWelcomeEmailAsync(string toEmail, string fullName, string temporaryPassword, string loginUrl, CancellationToken ct = default);
        // NEW: Calendar invite sender
        Task SendCalendarInviteAsync(string toEmail, string subject, string htmlBody, string icsContent, CancellationToken ct = default);
    }

}
