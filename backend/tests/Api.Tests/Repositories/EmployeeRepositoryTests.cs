using Api.Data;
using Api.Domain;
using Api.DTOs.Employees;
using Api.Repositories;
using Api.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Api.Tests.Repositories;

/// <summary>
/// Unit tests for EmployeeRepository data access logic.
/// Uses EF Core InMemory provider with a unique database per test class for isolation.
/// </summary>
public sealed class EmployeeRepositoryTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly IEmployeeRepository _repository;

    public EmployeeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _repository = new EmployeeRepository(_dbContext);

        SeedTestData();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    // ---------------------------------------------------------------------------
    // Seed helper — creates 5 employees matching the AppDbContext HasData records
    // ---------------------------------------------------------------------------

    private void SeedTestData()
    {
        _dbContext.Employees.AddRange(
            TestDataBuilders.BuildEmployee(1, "Alice Johnson", "alice.johnson@example.com", "Engineering", "Senior Software Engineer", true),
            TestDataBuilders.BuildEmployee(2, "Bob Martinez", "bob.martinez@example.com", "Engineering", "Software Engineer", true),
            TestDataBuilders.BuildEmployee(3, "Carol White", "carol.white@example.com", "Product", "Product Manager", true),
            TestDataBuilders.BuildEmployee(4, "David Lee", "david.lee@example.com", "Design", "UX Designer", true),
            TestDataBuilders.BuildEmployee(5, "Eva Brown", "eva.brown@example.com", "HR", "HR Manager", false)
        );
        _dbContext.SaveChanges();
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WithNoFilters_ReturnsAllEmployees()
    {
        // Arrange
        var query = new EmployeeQuery { Offset = 0, Limit = 100 };

        // Act
        var result = await _repository.GetAllAsync(query, CancellationToken.None);

        // Assert
        Assert.Equal(5, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WithDepartmentFilter_ReturnsOnlyMatchingEmployees()
    {
        // Arrange
        var query = new EmployeeQuery { Department = "Engineering", Offset = 0, Limit = 100 };

        // Act
        var result = await _repository.GetAllAsync(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, emp => Assert.Equal("Engineering", emp.Department));
    }

    [Fact]
    public async Task GetAllAsync_WithDepartmentFilter_IsCaseInsensitive()
    {
        // Arrange — lowercase filter should match "Engineering" stored values
        var query = new EmployeeQuery { Department = "engineering", Offset = 0, Limit = 100 };

        // Act
        var result = await _repository.GetAllAsync(query, CancellationToken.None);

        // Assert — case-insensitive filtering returns matching employees
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WithNullDepartment_ReturnsAllEmployees()
    {
        // Arrange
        var query = new EmployeeQuery { Department = null, Offset = 0, Limit = 100 };

        // Act
        var result = await _repository.GetAllAsync(query, CancellationToken.None);

        // Assert
        Assert.Equal(5, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyDepartment_ReturnsAllEmployees()
    {
        // Arrange
        var query = new EmployeeQuery { Department = "", Offset = 0, Limit = 100 };

        // Act
        var result = await _repository.GetAllAsync(query, CancellationToken.None);

        // Assert
        Assert.Equal(5, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WithActiveOnlyIndicatorTrue_ReturnsOnlyActiveEmployees()
    {
        // Arrange
        var query = new EmployeeQuery { ActiveOnlyIndicator = true, Offset = 0, Limit = 100 };

        // Act
        var result = await _repository.GetAllAsync(query, CancellationToken.None);

        // Assert
        Assert.Equal(4, result.Count());
        Assert.All(result, emp => Assert.True(emp.ActiveIndicator));
    }

    [Fact]
    public async Task GetAllAsync_WithActiveOnlyIndicatorFalse_ReturnsAllEmployees()
    {
        // Arrange
        var query = new EmployeeQuery { ActiveOnlyIndicator = false, Offset = 0, Limit = 100 };

        // Act
        var result = await _repository.GetAllAsync(query, CancellationToken.None);

        // Assert
        Assert.Equal(5, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var query = new EmployeeQuery { Offset = 2, Limit = 2 };

        // Act
        var result = await _repository.GetAllAsync(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WithOffsetBeyondResults_ReturnsEmptyCollection()
    {
        // Arrange
        var query = new EmployeeQuery { Offset = 100, Limit = 20 };

        // Act
        var result = await _repository.GetAllAsync(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WithDepartmentAndActiveFilters_ReturnsCombinedFilterResults()
    {
        // Arrange
        var query = new EmployeeQuery
        {
            Department = "Engineering",
            ActiveOnlyIndicator = true,
            Offset = 0,
            Limit = 100
        };

        // Act
        var result = await _repository.GetAllAsync(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, emp =>
        {
            Assert.Equal("Engineering", emp.Department);
            Assert.True(emp.ActiveIndicator);
        });
    }

    #endregion

    #region CountAsync Tests

    [Fact]
    public async Task CountAsync_WithNoFilters_ReturnsAllEmployeeCount()
    {
        // Arrange
        var query = new EmployeeQuery();

        // Act
        var count = await _repository.CountAsync(query, CancellationToken.None);

        // Assert
        Assert.Equal(5, count);
    }

    [Fact]
    public async Task CountAsync_WithDepartmentFilter_ReturnsFilteredCount()
    {
        // Arrange
        var query = new EmployeeQuery { Department = "Engineering" };

        // Act
        var count = await _repository.CountAsync(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task CountAsync_WithActiveOnlyIndicator_ReturnsActiveCount()
    {
        // Arrange
        var query = new EmployeeQuery { ActiveOnlyIndicator = true };

        // Act
        var count = await _repository.CountAsync(query, CancellationToken.None);

        // Assert
        Assert.Equal(4, count);
    }

    [Fact]
    public async Task CountAsync_WithNonExistentDepartment_ReturnsZero()
    {
        // Arrange
        var query = new EmployeeQuery { Department = "Finance" };

        // Act
        var count = await _repository.CountAsync(query, CancellationToken.None);

        // Assert
        Assert.Equal(0, count);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCorrectEmployee()
    {
        // Arrange
        var targetId = 1;

        // Act
        var result = await _repository.GetByIdAsync(targetId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(targetId, result.Id);
        Assert.Equal("Alice Johnson", result.FullName);
        Assert.Equal("Engineering", result.Department);
    }

    [Fact]
    public async Task GetByIdAsync_WithAnotherValidId_ReturnsCorrectEmployee()
    {
        // Arrange
        var targetId = 5;

        // Act
        var result = await _repository.GetByIdAsync(targetId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(targetId, result.Id);
        Assert.Equal("Eva Brown", result.FullName);
        Assert.False(result.ActiveIndicator);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var nonExistentId = 99999;

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithZeroId_ReturnsNull()
    {
        // Arrange
        var zeroId = 0;

        // Act
        var result = await _repository.GetByIdAsync(zeroId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    #endregion
}
