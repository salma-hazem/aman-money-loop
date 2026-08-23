using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Application.ServicesAbstractions.CircleRequestManagement;
using MonyLoop.Domain.Entities.CircleRequestManagement;
using MonyLoop.Domain.Entities.UserAuth;
using MonyLoop.Infrastructure.Services.Email;
using MonyLoop.Infrastructure.Services.Email.Models;

namespace MonyLoop.Infrastructure.Notifications;

public sealed class CircleRequestNotificationService : ICircleRequestNotificationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateRenderer _renderer;
    private readonly ILogger<CircleRequestNotificationService> _logger;

    public CircleRequestNotificationService(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        IEmailTemplateRenderer renderer,
        ILogger<CircleRequestNotificationService> logger)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _renderer = renderer;
        _logger = logger;
    }

    public async Task NotifySubmittedAsync(
        CircleRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var admins = await _userManager.GetUsersInRoleAsync(ApplicationRole.Admin);
            if (admins.Count == 0)
            {
                _logger.LogWarning("No Admin recipients were found for circle request {RequestId}.", request.RequestId);
                return;
            }

            foreach (var admin in admins.Where(user => !string.IsNullOrWhiteSpace(user.Email)))
            {
                var model = new CircleRequestSubmittedEmailModel
                {
                    RecipientName = GetDisplayName(admin, "Admin"),
                    RequestId = request.RequestId,
                    CircleTitle = request.CircleTitle,
                    CircleType = request.CircleType.ToString(),
                    SubmittedAt = request.SubmittedAt ?? request.CreatedAt
                };

                await TryRenderAndSendAsync(
                    "CircleRequestSubmittedEmail",
                    model,
                    admin.Email!,
                    "Circle request submitted",
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

            var model = new CircleRequestDecisionEmailModel
            {
                OrganizerName = GetDisplayName(organizer, "Organizer"),
                RequestId = request.RequestId,
                CircleTitle = request.CircleTitle,
                RequestStatus = request.RequestStatus.ToString(),
                DecisionReason = request.DecisionReason,
                ReviewedAt = request.ReviewedAt ?? request.SubmittedAt ?? request.CreatedAt
            };

            await TryRenderAndSendAsync(
                "CircleRequestDecisionEmail",
                model,
                organizer.Email,
                $"Circle request {request.RequestStatus}",
                request.RequestId,
                cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not notify the organizer for circle request {RequestId}.", request.RequestId);
        }
    }

    private async Task TryRenderAndSendAsync<TModel>(
        string templateName,
        TModel model,
        string recipient,
        string subject,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        try
        {
            var htmlBody = await _renderer.RenderAsync(templateName, model);
            await _emailSender.SendEmailAsync(recipient, subject, htmlBody, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Circle request email failed after commit for request {RequestId}.", requestId);
        }
    }

    private static string GetDisplayName(ApplicationUser user, string fallback)
    {
        var displayName = $"{user.FirstName} {user.LastName}".Trim();
        return string.IsNullOrWhiteSpace(displayName) ? fallback : displayName;
    }
}
