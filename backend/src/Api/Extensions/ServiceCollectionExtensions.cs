using Api.Repositories;
using Api.Services;

namespace Api.Extensions;

/// <summary>
/// Extension methods for configuring application services in the DI container.
/// Centralizes service registration following organization standards.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services, repositories, and dependencies.
    /// Call this method in Program.cs to configure the service container.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register repositories here using AddScoped
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        // Register services here using AddScoped
        services.AddScoped<IEmployeeService, EmployeeService>();

        return services;
    }
}
