# Repository DI Registration Guide

This guide defines the shared registration pattern for every module. Each team
member owns the repository interfaces and implementations for their module.
There is no base or generic repository requirement.

## 1. Shared Unit of Work

The application has one shared `IUnitOfWork` in `MonyLoop.Domain.Interfaces`.
It contains only `SaveChangesAsync`. Its implementation is in Infrastructure
and uses the shared `MonyLoopDbContext`.

`Program.cs` registers it once:

```csharp
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
```

Do not add module repository properties to `IUnitOfWork`. A service receives
the repositories it needs directly, then calls `SaveChangesAsync` once after a
successful business operation.

## 2. Each Module Registers Its Own Repositories

Create one Infrastructure extension class for your module. Example for a
module called `ExampleModule`:

```csharp
using Microsoft.Extensions.DependencyInjection;
using MonyLoop.Domain.Interfaces.ExampleModule;

namespace MonyLoop.Infrastructure.Repositories.ExampleModule;

public static class ExampleModuleServiceCollectionExtensions
{
    public static IServiceCollection AddExampleModuleRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IExampleRepository, ExampleRepository>();
        return services;
    }
}
```

Use `AddScoped` because repositories and `MonyLoopDbContext` must share the
same database context for one HTTP request.

## 3. Register the Module in Program.cs

Add the module namespace:

```csharp
using MonyLoop.Infrastructure.Repositories.ExampleModule;
```

Then register the module once during startup:

```csharp
builder.Services.AddExampleModuleRepositories();
```

Keep this call near the other Infrastructure registrations. Do not register
the same interface separately in `Program.cs` and the module extension.

## 4. Use Repositories in Application Services

An application service injects only the repositories required by its use case,
plus `IUnitOfWork` when it changes data:

```csharp
public sealed class ExampleService : IExampleService
{
    private readonly IExampleRepository _examples;
    private readonly IUnitOfWork _unitOfWork;

    public ExampleService(
        IExampleRepository examples,
        IUnitOfWork unitOfWork)
    {
        _examples = examples;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(Example entity, CancellationToken cancellationToken)
    {
        await _examples.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
```

## Checklist Before Opening a Pull Request

- Repository interface is in `Domain/Interfaces/<ModuleName>`.
- Repository implementation is in `Infrastructure/Repositories/<ModuleName>`.
- The module has one `Add<ModuleName>Repositories` extension method.
- `Program.cs` calls the module extension once.
- Application services use repository interfaces, never `DbContext` directly.
- Write operations call `IUnitOfWork.SaveChangesAsync` once after repository work.
- Build and tests pass.
