using Microsoft.EntityFrameworkCore;
using MonyLoop.Application.ServicesAbstractions;
using MonyLoop.Infrastructure.Data;
using MonyLoop.Infrastructure.DataSeeding;

namespace MonyLoop.API.Extentions
{
    public static class WebApplicationExtensions
    {
        public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MonyLoopDbContext>();

            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
                await dbContext.Database.MigrateAsync();

            return app;
        }

        public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dataInitializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>();
            await dataInitializer.InitializeAsync();

            var demoDataInitializer = scope.ServiceProvider.GetRequiredService<DemoDataInitializer>();
            await demoDataInitializer.InitializeAsync();

            return app;
        }
    }
}
