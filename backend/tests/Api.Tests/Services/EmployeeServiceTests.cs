using Api.Domain;
using Api.DTOs.Common;
using Api.DTOs.Employees;
using Api.Repositories;
using Api.Services;
using Moq;
using Xunit;

namespace Api.Tests.Services;

/// <summary>
/// Unit tests for <see cref="EmployeeService"/>.
/// </summary>
public sealed class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly EmployeeService _employeeService;

    public EmployeeServiceTests()
    {
        _employeeRepositoryMock = new Mock<IEmployeeRepository>(MockBehavior.Strict);
        _employeeService = new EmployeeService(_employeeRepositoryMock.Object);
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WhenRepositoryReturnsItems_ReturnsCollectionEnvelope()
    {
        // Arrange
        var query = new EmployeeQuery { Offset = 0, Limit = 20 };
        var selfUrl = "http://localhost/v1/employees";
        var employees = new[] { BuildEmployee(1, "Alice"), BuildEmployee(2, "Bob") };

        _employeeRepositoryMock
            .Setup(repo => repo.GetAllAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);
        _employeeRepositoryMock
            .Setup(repo => repo.CountAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees.Length);

        // Act
        var collectionEnvelope = await _employeeService.GetAllAsync(query, selfUrl, CancellationToken.None);

        // Assert
        Assert.NotNull(collectionEnvelope);
        Assert.Equal(2, collectionEnvelope.Items.Count());
        Assert.Equal(2, collectionEnvelope.Metadata.TotalCount);
        Assert.Equal(selfUrl, collectionEnvelope.Links.Self);
        Assert.False(string.IsNullOrWhiteSpace(collectionEnvelope.Metadata.TransactionId));
        _employeeRepositoryMock.Verify(repo => repo.GetAllAsync(query, It.IsAny<CancellationToken>()), Times.Once());
        _employeeRepositoryMock.Verify(repo => repo.CountAsync(query, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetAllAsync_WhenRepositoryReturnsEmpty_ReturnsEmptyCollectionEnvelope()
    {
        // Arrange
        var query = new EmployeeQuery();
        var selfUrl = "http://localhost/v1/employees";

        _employeeRepositoryMock
            .Setup(repo => repo.GetAllAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Employee>());
        _employeeRepositoryMock
            .Setup(repo => repo.CountAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var collectionEnvelope = await _employeeService.GetAllAsync(query, selfUrl, CancellationToken.None);

        // Assert
        Assert.Empty(collectionEnvelope.Items);
        Assert.Equal(0, collectionEnvelope.Metadata.TotalCount);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsItemEnvelope()
    {
        // Arrange
        var employee = BuildEmployee(42, "Jane Doe");
        var selfUrl = "http://localhost/v1/employees/42";

        _employeeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        // Act
        var itemEnvelope = await _employeeService.GetByIdAsync(42, selfUrl, CancellationToken.None);

        // Assert
        Assert.NotNull(itemEnvelope);
        Assert.Equal(42, itemEnvelope!.Item.EmployeeId);
        Assert.Equal("Jane Doe", itemEnvelope.Item.Name);
        Assert.Equal(selfUrl, itemEnvelope.Links.Self);
        Assert.Null(itemEnvelope.Metadata.TotalCount);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeMissing_ReturnsNull()
    {
        // Arrange
        _employeeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        // Act
        var itemEnvelope = await _employeeService.GetByIdAsync(99, "http://localhost/v1/employees/99", CancellationToken.None);

        // Assert
        Assert.Null(itemEnvelope);
    }

    #endregion

    private static Employee BuildEmployee(int employeeId, string name)
    {
        return new Employee
        {
            EmployeeId = employeeId,
            Name = name,
            Email = $"{name.Replace(' ', '.').ToLowerInvariant()}@org.com",
            Department = "ENG",
            HireDate = new DateTime(2020, 1, 1),
            ActiveIndicator = true,
            Salary = 90000m,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };
    }
}
