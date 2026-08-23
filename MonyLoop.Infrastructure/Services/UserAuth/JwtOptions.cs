using System.ComponentModel.DataAnnotations;

namespace MonyLoop.Infrastructure.Services.UserAuth;

public sealed class JwtOptions
{
    public const string SectionName = "JwtOptions";

    [Required]
    [MinLength(32)]
    public string SecretKey { get; init; } = string.Empty;

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Range(1, 1440)]
    public int AccessTokenExpiryMinutes { get; init; } = 15;

    [Range(1, 365)]
    public int RefreshTokenExpiryDays { get; init; } = 7;
}
