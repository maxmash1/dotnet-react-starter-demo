using Api.Domain;
using Api.DTOs.Common;
using Api.DTOs.Employees;
using Api.Repositories;
using Api.Services;
using Api.Tests.Utils;
using Moq;
using Xunit;

namespace Api.Tests.Services;

/// <summary>
/// Unit tests for EmployeeService business logic.
/// Uses Moq to isolate the service from the repository layer.
/// </summary>
public sealed class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly IEmployeeService _service;

    public EmployeeServiceTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _service = new EmployeeService(_repositoryMock.Object);
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WhenRepositoryReturnsEmployees_ReturnsCollectionEnvelope()
    {
        // Arrange
        var query = new EmployeeQuery { Offset = 0, Limit = 20 };
        var employees = new List<Employee>
        {
            TestDataBuilders.BuildEmployee(1, "Alice Johnson", "alice@example.com", "Engineering", "Engineer", true),
            TestDataBuilders.BuildEmployee(2, "Bob Martinez", "bob@example.com", "Product", "Manager", true)
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        _repositoryMock
            .Setup(r => r.CountAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees.Count);

        // Act
        var result = await _service.GetAllAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CollectionResponseDto<EmployeeDto>>(result);
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ReturnsMetadataWithTotalCount()
    {
        // Arrange
        var query = new EmployeeQuery { Offset = 0, Limit = 20 };
        var employees = new List<Employee>
        {
            TestDataBuilders.BuildEmployee(1),
            TestDataBuilders.BuildEmployee(2, "Jane Doe", "jane@example.com")
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        _repositoryMock
            .Setup(r => r.CountAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5); // total is larger than current page

        // Act
        var result = await _service.GetAllAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Metadata);
        Assert.Equal(5, result.Metadata.TotalCount);
        Assert.False(string.IsNullOrWhiteSpace(result.Metadata.TransactionId));
        Assert.True(result.Metadata.Timestamp <= DateTime.UtcNow);
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ReturnsSelfLinkWithOffsetAndLimit()
    {
        // Arrange
        var query = new EmployeeQuery { Offset = 0, Limit = 20 };

        _repositoryMock
            .Setup(r => r.GetAllAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Employee>());

        _repositoryMock
            .Setup(r => r.CountAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _service.GetAllAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Links);
        Assert.Contains("offset=0", result.Links.Self);
        Assert.Contains("limit=20", result.Links.Self);
    }

    [Fact]
    public async Task GetAllAsync_WhenMorePagesBeyondCurrent_ReturnsNextLink()
    {
        // Arrange
        var query = new EmployeeQuery { Offset = 0, Limit = 2 };

        _repositoryMock
            .Setup(r => r.GetAllAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Employee>
            {
                TestDataBuilders.BuildEmployee(1),
                TestDataBuilders.BuildEmployee(2, "Jane Doe", "jane@example.com")
            });

        _repositoryMock
            .Setup(r => r.CountAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5); // 5 total > 0 + 2, so next page exists

        // Act
        var result = await _service.GetAllAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Links.Next);
        Assert.Contains("offset=2", result.Links.Next);
        Assert.Contains("limit=2", result.Links.Next);
    }

    [Fact]
    public async Task GetAllAsync_WhenOnFirstPage_ReturnsPrevLinkAsNull()
    {
        // Arrange
        var query = new EmployeeQuery { Offset = 0, Limit = 20 };

        _repositoryMock
            .Setup(r => r.GetAllAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Employee>());

        _repositoryMock
            .Setup(r => r.CountAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _service.GetAllAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result.Links.Prev);
    }

    [Fact]
    public async Task GetAllAsync_WhenOnSecondPage_ReturnsPrevLink()
    {
        // Arrange
        var query = new EmployeeQuery { Offset = 20, Limit = 20 };

        _repositoryMock
            .Setup(r => r.GetAllAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Employee>());

        _repositoryMock
            .Setup(r => r.CountAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(25);

        // Act
        var result = await _service.GetAllAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Links.Prev);
        Assert.Contains("offset=0", result.Links.Prev);
        Assert.Contains("limit=20", result.Links.Prev);
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_MapsEntitiesToDtosCorrectly()
    {
        // Arrange
        var query = new EmployeeQuery { Offset = 0, Limit = 20 };
        var employee = TestDataBuilders.BuildEmployee(
            id: 42,
            fullName: "Carol White",
            email: "carol@example.com",
            department: "Product",
            jobTitle: "Product Manager",
            active: true);

        _repositoryMock
            .Setup(r => r.GetAllAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Employee> { employee });

        _repositoryMock
            .Setup(r => r.CountAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.GetAllAsync(query, CancellationToken.None);
        var dto = result.Items.Single();

        // Assert
        Assert.Equal(42, dto.EmployeeId);
        Assert.Equal("Carol White", dto.FullName);
        Assert.Equal("carol@example.com", dto.Email);
        Assert.Equal("Product", dto.Department);
        Assert.Equal("Product Manager", dto.JobTitle);
        Assert.True(dto.ActiveIndicator);
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_InvokesRepositoryOnce()
    {
        // Arrange
        var query = new EmployeeQuery { Offset = 0, Limit = 20 };

        _repositoryMock
            .Setup(r => r.GetAllAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Employee>());

        _repositoryMock
            .Setup(r => r.CountAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        await _service.GetAllAsync(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetAllAsync(query, It.IsAny<CancellationToken>()), Times.Once());
        _repositoryMock.Verify(r => r.CountAsync(query, It.IsAny<CancellationToken>()), Times.Once());
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsItemEnvelope()
    {
        // Arrange
        var employee = TestDataBuilders.BuildEmployee(id: 7, fullName: "David Lee", email: "david@example.com");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        // Act
        var result = await _service.GetByIdAsync(7, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ItemResponseDto<EmployeeDto>>(result);
        Assert.Equal(7, result.Item.EmployeeId);
        Assert.Equal("David Lee", result.Item.FullName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsMetadataWithTransactionId()
    {
        // Arrange
        var employee = TestDataBuilders.BuildEmployee(id: 3);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        // Act
        var result = await _service.GetByIdAsync(3, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Metadata);
        Assert.False(string.IsNullOrWhiteSpace(result.Metadata.TransactionId));
        Assert.Null(result.Metadata.TotalCount);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsSelfLinkWithId()
    {
        // Arrange
        var employee = TestDataBuilders.BuildEmployee(id: 5);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        // Act
        var result = await _service.GetByIdAsync(5, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("/v1/employees/5", result.Links.Self);
        Assert.Null(result.Links.Next);
        Assert.Null(result.Links.Prev);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(99999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        // Act
        var result = await _service.GetByIdAsync(99999, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeDoesNotExist_InvokesRepositoryOnce()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        // Act
        await _service.GetByIdAsync(42, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()), Times.Once());
    }

    #endregion
}
