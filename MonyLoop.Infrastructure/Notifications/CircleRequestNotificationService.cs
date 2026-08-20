using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MonyLoop.Application.ServicesAbstractions;
using MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Entities.CircleRequestManagement;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.Infrastructure.Notifications;

public sealed class CircleRequestNotificationService : ICircleRequestNotificationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<CircleRequestNotificationService> _logger;

    public CircleRequestNotificationService(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        ILogger<CircleRequestNotificationService> logger)
    {
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task NotifySubmittedAsync(
        CircleRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var admins = await _userManager.GetUsersInRoleAsync(SystemRoles.Admin);
            foreach (var admin in admins.Where(user => !string.IsNullOrWhiteSpace(user.Email)))
            {
                await TrySendAsync(
                    admin.Email!,
                    "Circle request submitted",
                    $"<p>Circle request <strong>{WebUtility.HtmlEncode(request.CircleTitle)}</strong> is ready for review.</p>",
                    request.RequestId,
                    cancellationToken);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not resolve Admin recipients for circle request {RequestId}.", request.RequestId);
        }
    }

    public async Task NotifyDecisionAsync(
        CircleRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var organizer = await _userManager.FindByIdAsync(request.CreatedByOrganizerId.ToString());
            if (string.IsNullOrWhiteSpace(organizer?.Email))
            {
                _logger.LogWarning("Organizer email was not found for circle request {RequestId}.", request.RequestId);
                return;
            }

            var reason = string.IsNullOrWhiteSpace(request.DecisionReason)
                ? string.Empty
                : $"<p>Reason: {WebUtility.HtmlEncode(request.DecisionReason)}</p>";

            await TrySendAsync(
                organizer.Email,
                $"Circle request {request.RequestStatus}",
                $"<p>Your request <strong>{WebUtility.HtmlEncode(request.CircleTitle)}</strong> is now <strong>{request.RequestStatus}</strong>.</p>{reason}",
                request.RequestId,
                cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not notify the organizer for circle request {RequestId}.", request.RequestId);
        }
    }

    private async Task TrySendAsync(
        string recipient,
        string subject,
        string htmlBody,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _emailService.SendEmailAsync(recipient, subject, htmlBody, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Circle request email failed after commit for request {RequestId}.", requestId);
        }
    }
}
