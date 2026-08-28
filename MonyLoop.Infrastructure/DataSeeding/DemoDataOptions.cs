namespace MonyLoop.Infrastructure.DataSeeding;

public sealed class DemoDataOptions
{
    public const string SectionName = "DemoData";

    public bool Enabled { get; init; }
    public string Password { get; init; } = string.Empty;
}
