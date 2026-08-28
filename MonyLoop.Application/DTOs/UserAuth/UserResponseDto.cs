namespace MonyLoop.Application.DTOs.UserAuth;

public sealed class UserResponseDto
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? NationalId { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool MustChangePassword { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = [];
}
