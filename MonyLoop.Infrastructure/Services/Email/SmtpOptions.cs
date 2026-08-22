using System.ComponentModel.DataAnnotations;

namespace MonyLoop.Infrastructure.Services.Email;

public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";

    [Required]
    public string Host { get; init; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; init; } = 587;

    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string FromEmail { get; init; } = string.Empty;

    [Required]
    public string FromName { get; init; } = "Aman Money Loop";

    public bool EnableSsl { get; init; } = true;
}
