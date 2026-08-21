using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MonyLoop.Application.ServicesAbstractions;
using MonyLoop.Domain.Entities.UserAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Infrastructure.DataSeeding
{
    public class IdentityDataInitializer : IDataInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<IdentityDataInitializer> _logger;

        public IdentityDataInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<IdentityDataInitializer> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (!_roleManager.Roles.Any())
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = ApplicationRole.Admin });
                    await _roleManager.CreateAsync(new ApplicationRole { Name = ApplicationRole.Organizer });
                    await _roleManager.CreateAsync(new ApplicationRole { Name = ApplicationRole.Member });
                }

                if (!_userManager.Users.Any())
                {
                    var adminUser = new ApplicationUser
                    {
                        Id = Guid.NewGuid(),
                        UserName = "admin@monyloop.com",
                        Email = "admin@monyloop.com",
                        FirstName = "System",
                        LastName = "Admin",
                        NationalId = "00000000000000",
                        EmailConfirmed = true,
                        MustChangePassword = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    var result = await _userManager.CreateAsync(adminUser, "Admin@123");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(adminUser, ApplicationRole.Admin);
                        _logger.LogInformation("Initial Admin user created successfully.");
                    }
                    else
                    {
                        _logger.LogError("Failed to create initial Admin user: {Errors}",
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while seeding Identity database.");
            }
        }
    }
}
