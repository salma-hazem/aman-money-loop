using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MonyLoop.Application.ServicesAbstractions;
using MonyLoop.Domain.Entities.UserAuth;

namespace MonyLoop.Infrastructure.DataSeeding;

public sealed class IdentityDataInitializer : IDataInitializer
{
    private static readonly string[] RequiredRoles =
    [
        ApplicationRole.Admin,
        ApplicationRole.Organizer,
        ApplicationRole.Member
    ];

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SeedAdminOptions _options;
    private readonly ILogger<IdentityDataInitializer> _logger;

    public IdentityDataInitializer(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<SeedAdminOptions> options,
        ILogger<IdentityDataInitializer> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _options = options.Value;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            await EnsureRolesAsync();
            await EnsureInitialAdminAsync();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while seeding Identity data.");
        }
    }

    private async Task EnsureRolesAsync()
    {
        foreach (var roleName in RequiredRoles)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            if (!result.Succeeded)
            {
                _logger.LogError(
                    "Failed to create role {Role}: {Errors}",
                    roleName,
                    string.Join(", ", result.Errors.Select(error => error.Description)));
            }
        }
    }

    private async Task EnsureInitialAdminAsync()
    {
        if (string.IsNullOrWhiteSpace(_options.Email) ||
            string.IsNullOrWhiteSpace(_options.Password))
        {
            _logger.LogWarning(
                "Initial Admin was not seeded because SeedAdmin:Email or SeedAdmin:Password is missing.");
            return;
        }

        var email = _options.Email.Trim();
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            if (!await _userManager.IsInRoleAsync(existingUser, ApplicationRole.Admin))
            {
                await _userManager.AddToRoleAsync(existingUser, ApplicationRole.Admin);
            }

            return;
        }

        var admin = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            FirstName = string.IsNullOrWhiteSpace(_options.FirstName)
                ? "System"
                : _options.FirstName.Trim(),
            LastName = string.IsNullOrWhiteSpace(_options.LastName)
                ? "Admin"
                : _options.LastName.Trim(),
            NationalId = null,
            EmailConfirmed = true,
            MustChangePassword = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(admin, _options.Password);
        if (!createResult.Succeeded)
        {
            _logger.LogError(
                "Failed to create the initial Admin: {Errors}",
                string.Join(", ", createResult.Errors.Select(error => error.Description)));
            return;
        }

        var roleResult = await _userManager.AddToRoleAsync(admin, ApplicationRole.Admin);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(admin);
            _logger.LogError(
                "Failed to assign the Admin role: {Errors}",
                string.Join(", ", roleResult.Errors.Select(error => error.Description)));
            return;
        }

        _logger.LogInformation("Initial Admin {Email} was created.", email);
    }
}
