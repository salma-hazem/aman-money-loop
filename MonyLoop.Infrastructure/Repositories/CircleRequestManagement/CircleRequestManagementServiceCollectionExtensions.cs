using Microsoft.Extensions.DependencyInjection;
using MonyLoop.Domain.Interfaces.CircleRequestManagement;

namespace MonyLoop.Infrastructure.Repositories.CircleRequestManagement;

public static class CircleRequestManagementServiceCollectionExtensions
{
    public static IServiceCollection AddCircleRequestManagementRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<ICircleRequestRepository, CircleRequestRepository>();
        services.AddScoped<ICircleRepository, CircleRepository>();
        services.AddScoped<IMarketplaceListingRepository,
            MarketplaceListingRepository>();
        services.AddScoped<ICircleSlotRepository, CircleSlotRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        return services;
    }
}
