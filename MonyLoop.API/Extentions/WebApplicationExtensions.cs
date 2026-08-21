using Microsoft.EntityFrameworkCore;
using MonyLoop.Application.ServicesAbstractions;
using MonyLoop.Infrastructure.Data;

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

            return app;
        }
    }
}
