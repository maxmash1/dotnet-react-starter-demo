using System.Net;
using System.Net.Http.Json;
using Api.Data;
using Api.Domain;
using Api.DTOs.Common;
using Api.DTOs.Employees;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Api.Tests.Controllers;

/// <summary>
/// Integration tests for <see cref="Api.Controllers.EmployeesController"/>.
/// Replaces the production InMemory database with an isolated test database and seeds employees.
/// </summary>
public sealed class EmployeesControllerTests : IClassFixture<EmployeesControllerTests.SeededFactory>
{
    private readonly HttpClient _httpClient;

    public EmployeesControllerTests(SeededFactory applicationFactory)
    {
        _httpClient = applicationFactory.CreateClient();
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WhenCalledWithoutFilters_ReturnsOkWithCollectionEnvelope()
    {
        // Arrange
        var endpointPath = "/v1/employees";

        // Act
        var httpResponse = await _httpClient.GetAsync(endpointPath);
        var collectionEnvelope = await httpResponse.Content.ReadFromJsonAsync<CollectionResponseDto<EmployeeDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.NotNull(collectionEnvelope);
        Assert.Equal(3, collectionEnvelope!.Items.Count());
        Assert.Equal(3, collectionEnvelope.Metadata.TotalCount);
        Assert.Contains("/v1/employees", collectionEnvelope.Links.Self);
    }

    [Fact]
    public async Task GetAllAsync_WhenDepartmentFilterApplied_ReturnsOnlyMatchingEmployees()
    {
        // Arrange
        var endpointPath = "/v1/employees?department=ENG";

        // Act
        var collectionEnvelope = await _httpClient.GetFromJsonAsync<CollectionResponseDto<EmployeeDto>>(endpointPath);

        // Assert
        Assert.NotNull(collectionEnvelope);
        Assert.All(collectionEnvelope!.Items, employee => Assert.Equal("ENG", employee.Department));
        Assert.Equal(2, collectionEnvelope.Items.Count());
    }

    [Fact]
    public async Task GetAllAsync_WhenLimitProvided_RespectsPagination()
    {
        // Arrange
        var endpointPath = "/v1/employees?offset=0&limit=1";

        // Act
        var collectionEnvelope = await _httpClient.GetFromJsonAsync<CollectionResponseDto<EmployeeDto>>(endpointPath);

        // Assert
        Assert.NotNull(collectionEnvelope);
        Assert.Single(collectionEnvelope!.Items);
        Assert.Equal(3, collectionEnvelope.Metadata.TotalCount);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsOkWithItemEnvelope()
    {
        // Arrange
        var endpointPath = "/v1/employees/1";

        // Act
        var httpResponse = await _httpClient.GetAsync(endpointPath);
        var itemEnvelope = await httpResponse.Content.ReadFromJsonAsync<ItemResponseDto<EmployeeDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.NotNull(itemEnvelope);
        Assert.Equal(1, itemEnvelope!.Item.EmployeeId);
        Assert.Equal("Alice Johnson", itemEnvelope.Item.Name);
        Assert.True(itemEnvelope.Item.ActiveIndicator);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeMissing_ReturnsNotFoundWithErrorEnvelope()
    {
        // Arrange
        var endpointPath = "/v1/employees/9999";

        // Act
        var httpResponse = await _httpClient.GetAsync(endpointPath);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
        var errorEnvelope = await httpResponse.Content.ReadFromJsonAsync<ErrorResponseDto>();
        Assert.NotNull(errorEnvelope);
        Assert.Equal("ORG-NTF-001", errorEnvelope!.Code);
    }

    #endregion

    /// <summary>
    /// Test factory that swaps the production DbContext for an isolated InMemory database
    /// and seeds a deterministic set of employees.
    /// </summary>
    public sealed class SeededFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = $"EmployeesTestDb_{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.UseEnvironment("Testing");

            webHostBuilder.ConfigureServices(serviceCollection =>
            {
                var existingDbDescriptor = serviceCollection.SingleOrDefault(
                    descriptor => descriptor.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (existingDbDescriptor != null)
                {
                    serviceCollection.Remove(existingDbDescriptor);
                }

                serviceCollection.AddDbContext<AppDbContext>(dbContextOptions =>
                    dbContextOptions.UseInMemoryDatabase(_databaseName));

                using var scopedProvider = serviceCollection.BuildServiceProvider().CreateScope();
                var seededDbContext = scopedProvider.ServiceProvider.GetRequiredService<AppDbContext>();
                SeedEmployees(seededDbContext);
            });
        }

        private static void SeedEmployees(AppDbContext dbContext)
        {
            if (dbContext.Employees.Any())
            {
                return;
            }

            dbContext.Employees.AddRange(
                new Employee
                {
                    EmployeeId = 1,
                    Name = "Alice Johnson",
                    Email = "alice@org.com",
                    Department = "ENG",
                    HireDate = new DateTime(2020, 1, 15),
                    ActiveIndicator = true,
                    Salary = 95000m,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                },
                new Employee
                {
                    EmployeeId = 2,
                    Name = "Bob Martinez",
                    Email = "bob@org.com",
                    Department = "ENG",
                    HireDate = new DateTime(2019, 6, 20),
                    ActiveIndicator = true,
                    Salary = 102000m,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                },
                new Employee
                {
                    EmployeeId = 3,
                    Name = "Carol Williams",
                    Email = "carol@org.com",
                    Department = "SAL",
                    HireDate = new DateTime(2021, 3, 10),
                    ActiveIndicator = false,
                    Salary = 78000m,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                });

            dbContext.SaveChanges();
        }
    }
}
