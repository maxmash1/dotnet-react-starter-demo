using Api.Data;
using Api.Domain;
using Api.DTOs.Employees;
using Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Api.Tests.Repositories;

/// <summary>
/// Unit tests for <see cref="EmployeeRepository"/> using EF Core InMemory provider.
/// </summary>
public sealed class EmployeeRepositoryTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly EmployeeRepository _employeeRepository;

    public EmployeeRepositoryTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(dbContextOptions);
        _employeeRepository = new EmployeeRepository(_dbContext);

        SeedEmployees();
    }

    public void Dispose() => _dbContext.Dispose();

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsEmployee()
    {
        // Arrange
        var existingEmployeeId = 1;

        // Act
        var foundEmployee = await _employeeRepository.GetByIdAsync(existingEmployeeId, CancellationToken.None);

        // Assert
        Assert.NotNull(foundEmployee);
        Assert.Equal(existingEmployeeId, foundEmployee!.EmployeeId);
        Assert.Equal("Alice Johnson", foundEmployee.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeMissing_ReturnsNull()
    {
        // Arrange
        var missingEmployeeId = 9999;

        // Act
        var foundEmployee = await _employeeRepository.GetByIdAsync(missingEmployeeId, CancellationToken.None);

        // Assert
        Assert.Null(foundEmployee);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WhenNoFiltersApplied_ReturnsAllEmployeesPaged()
    {
        // Arrange
        var unfilteredQuery = new EmployeeQuery { Offset = 0, Limit = 20 };

        // Act
        var employeeResults = await _employeeRepository.GetAllAsync(unfilteredQuery, CancellationToken.None);

        // Assert
        Assert.Equal(3, employeeResults.Count());
    }

    [Fact]
    public async Task GetAllAsync_WhenDepartmentFilterApplied_ReturnsOnlyMatchingDepartment()
    {
        // Arrange
        var engineeringQuery = new EmployeeQuery { Department = "ENG" };

        // Act
        var employeeResults = await _employeeRepository.GetAllAsync(engineeringQuery, CancellationToken.None);

        // Assert
        Assert.All(employeeResults, employee => Assert.Equal("ENG", employee.Department));
        Assert.Equal(2, employeeResults.Count());
    }

    [Fact]
    public async Task GetAllAsync_WhenDepartmentFilterIsEmptyString_ReturnsAllEmployees()
    {
        // Arrange — empty string means "no filter"
        var noDepartmentQuery = new EmployeeQuery { Department = string.Empty };

        // Act
        var employeeResults = await _employeeRepository.GetAllAsync(noDepartmentQuery, CancellationToken.None);

        // Assert
        Assert.Equal(3, employeeResults.Count());
    }

    [Fact]
    public async Task GetAllAsync_WhenActiveFilterIsTrue_ReturnsOnlyActiveEmployees()
    {
        // Arrange
        var activeOnlyQuery = new EmployeeQuery { ActiveIndicator = true };

        // Act
        var employeeResults = await _employeeRepository.GetAllAsync(activeOnlyQuery, CancellationToken.None);

        // Assert
        Assert.All(employeeResults, employee => Assert.True(employee.ActiveIndicator));
        Assert.Equal(2, employeeResults.Count());
    }

    [Fact]
    public async Task GetAllAsync_WhenActiveFilterNotProvided_ReturnsActiveAndInactive()
    {
        // Arrange — null active indicator means "no filter"
        var noActiveFilterQuery = new EmployeeQuery { ActiveIndicator = null };

        // Act
        var employeeResults = await _employeeRepository.GetAllAsync(noActiveFilterQuery, CancellationToken.None);

        // Assert
        Assert.Equal(3, employeeResults.Count());
    }

    [Fact]
    public async Task GetAllAsync_WhenLimitProvided_RespectsPaginationLimit()
    {
        // Arrange
        var pagedQuery = new EmployeeQuery { Offset = 0, Limit = 1 };

        // Act
        var employeeResults = await _employeeRepository.GetAllAsync(pagedQuery, CancellationToken.None);

        // Assert
        Assert.Single(employeeResults);
    }

    #endregion

    #region CountAsync Tests

    [Fact]
    public async Task CountAsync_WhenNoFiltersApplied_ReturnsTotalCount()
    {
        // Arrange
        var unfilteredQuery = new EmployeeQuery();

        // Act
        var totalCount = await _employeeRepository.CountAsync(unfilteredQuery, CancellationToken.None);

        // Assert
        Assert.Equal(3, totalCount);
    }

    [Fact]
    public async Task CountAsync_WhenDepartmentFilterApplied_ReturnsFilteredCount()
    {
        // Arrange
        var engineeringQuery = new EmployeeQuery { Department = "ENG" };

        // Act
        var filteredCount = await _employeeRepository.CountAsync(engineeringQuery, CancellationToken.None);

        // Assert
        Assert.Equal(2, filteredCount);
    }

    #endregion

    private void SeedEmployees()
    {
        _dbContext.Employees.AddRange(
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

        _dbContext.SaveChanges();
    }
}
