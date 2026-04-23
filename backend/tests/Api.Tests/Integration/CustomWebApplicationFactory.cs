using Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api.Tests.Integration;

/// <summary>
/// Custom web application factory for integration testing.
/// Replaces the production InMemory database with an isolated test database,
/// ensures seed data is applied via CreateHost, and uses the Testing environment.
/// </summary>
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _testDatabaseName = $"IntegrationTestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder webHostBuilder)
    {
        webHostBuilder.UseEnvironment("Testing");

        webHostBuilder.ConfigureServices(serviceCollection =>
        {
            // Remove the production DbContext options so we can replace the database name
            var existingDbDescriptor = serviceCollection.SingleOrDefault(
                descriptor => descriptor.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (existingDbDescriptor != null)
                serviceCollection.Remove(existingDbDescriptor);

            // Register an isolated InMemory database unique to this factory instance
            serviceCollection.AddDbContext<AppDbContext>(dbOptions =>
                dbOptions.UseInMemoryDatabase(_testDatabaseName));
        });
    }

    /// <summary>
    /// Override CreateHost to seed test data using the real application service provider.
    /// This runs after the host is built so the actual DI container is used, not a temp one.
    /// </summary>
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        // Seed the isolated InMemory database using the host's actual service provider
        using var serviceScope = host.Services.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();

        return host;
    }
}
