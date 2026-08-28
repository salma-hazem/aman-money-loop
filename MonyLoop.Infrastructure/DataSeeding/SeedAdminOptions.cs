namespace MonyLoop.Infrastructure.DataSeeding;

public sealed class SeedAdminOptions
{
    public const string SectionName = "SeedAdmin";

    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = "System";
    public string LastName { get; init; } = "Admin";
}
