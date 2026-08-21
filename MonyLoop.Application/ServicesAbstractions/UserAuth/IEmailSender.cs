using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.ServicesAbstractions.UserAuth
{

    public interface IEmailSender
    {
        Task SendOtpEmailAsync(string toEmail, string userName, string code, int expiryMinutes, CancellationToken ct = default);
        Task SendWelcomeEmailAsync(string toEmail, string fullName, string temporaryPassword, string loginUrl, CancellationToken ct = default);
    }

}
