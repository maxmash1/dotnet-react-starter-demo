using System.Net;
using System.Net.Http.Json;
using Api.DTOs.Common;
using Api.DTOs.Employees;
using Api.Tests.Integration;
using Xunit;

namespace Api.Tests.Controllers;

/// <summary>
/// Integration tests for the EmployeesController endpoints.
/// Uses CustomWebApplicationFactory which seeds an isolated InMemory database.
/// </summary>
public sealed class EmployeesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _testClient;

    public EmployeesControllerTests(CustomWebApplicationFactory applicationFactory)
    {
        _testClient = applicationFactory.CreateClient();
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WhenCalled_ReturnsOkStatusCode()
    {
        // Arrange
        var endpoint = "/v1/employees";

        // Act
        var httpResponse = await _testClient.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ReturnsCollectionEnvelopeWithItems()
    {
        // Arrange
        var endpoint = "/v1/employees";

        // Act
        var envelope = await _testClient.GetFromJsonAsync<CollectionResponseDto<EmployeeDto>>(endpoint);

        // Assert
        Assert.NotNull(envelope);
        Assert.NotNull(envelope.Items);
        Assert.NotEmpty(envelope.Items);
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ReturnsMetadataWithTotalCount()
    {
        // Arrange
        var endpoint = "/v1/employees";

        // Act
        var envelope = await _testClient.GetFromJsonAsync<CollectionResponseDto<EmployeeDto>>(endpoint);

        // Assert
        Assert.NotNull(envelope);
        Assert.NotNull(envelope.Metadata);
        Assert.NotNull(envelope.Metadata.TotalCount);
        Assert.True(envelope.Metadata.TotalCount > 0);
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ReturnsMetadataWithTransactionId()
    {
        // Arrange
        var endpoint = "/v1/employees";

        // Act
        var envelope = await _testClient.GetFromJsonAsync<CollectionResponseDto<EmployeeDto>>(endpoint);

        // Assert
        Assert.NotNull(envelope);
        Assert.NotNull(envelope.Metadata);
        Assert.False(string.IsNullOrWhiteSpace(envelope.Metadata.TransactionId));
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ReturnsLinksWithSelfUrl()
    {
        // Arrange
        var endpoint = "/v1/employees";

        // Act
        var envelope = await _testClient.GetFromJsonAsync<CollectionResponseDto<EmployeeDto>>(endpoint);

        // Assert
        Assert.NotNull(envelope);
        Assert.NotNull(envelope.Links);
        Assert.Contains("/v1/employees", envelope.Links.Self);
    }

    [Fact]
    public async Task GetAllAsync_WithDepartmentFilter_ReturnsOnlyMatchingEmployees()
    {
        // Arrange
        var endpoint = "/v1/employees?department=Engineering";

        // Act
        var envelope = await _testClient.GetFromJsonAsync<CollectionResponseDto<EmployeeDto>>(endpoint);

        // Assert
        Assert.NotNull(envelope);
        Assert.NotNull(envelope.Items);
        Assert.All(envelope.Items, emp => Assert.Equal("Engineering", emp.Department));
    }

    [Fact]
    public async Task GetAllAsync_WithActiveOnlyFilter_ReturnsOnlyActiveEmployees()
    {
        // Arrange
        var endpoint = "/v1/employees?activeOnlyIndicator=true";

        // Act
        var envelope = await _testClient.GetFromJsonAsync<CollectionResponseDto<EmployeeDto>>(endpoint);

        // Assert
        Assert.NotNull(envelope);
        Assert.NotNull(envelope.Items);
        Assert.All(envelope.Items, emp => Assert.True(emp.ActiveIndicator));
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsOkStatusCode()
    {
        // Arrange
        var endpoint = "/v1/employees/1";

        // Act
        var httpResponse = await _testClient.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsItemEnvelopeWithEmployee()
    {
        // Arrange
        var endpoint = "/v1/employees/1";

        // Act
        var envelope = await _testClient.GetFromJsonAsync<ItemResponseDto<EmployeeDto>>(endpoint);

        // Assert
        Assert.NotNull(envelope);
        Assert.NotNull(envelope.Item);
        Assert.Equal(1, envelope.Item.EmployeeId);
        Assert.False(string.IsNullOrWhiteSpace(envelope.Item.FullName));
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsMetadataWithTransactionId()
    {
        // Arrange
        var endpoint = "/v1/employees/1";

        // Act
        var envelope = await _testClient.GetFromJsonAsync<ItemResponseDto<EmployeeDto>>(endpoint);

        // Assert
        Assert.NotNull(envelope);
        Assert.NotNull(envelope.Metadata);
        Assert.False(string.IsNullOrWhiteSpace(envelope.Metadata.TransactionId));
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsLinksWithSelfUrl()
    {
        // Arrange
        var endpoint = "/v1/employees/1";

        // Act
        var envelope = await _testClient.GetFromJsonAsync<ItemResponseDto<EmployeeDto>>(endpoint);

        // Assert
        Assert.NotNull(envelope);
        Assert.NotNull(envelope.Links);
        Assert.Contains("/v1/employees/1", envelope.Links.Self);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNotFoundStatusCode()
    {
        // Arrange
        var endpoint = "/v1/employees/99999";

        // Act
        var httpResponse = await _testClient.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsErrorResponseBody()
    {
        // Arrange
        var endpoint = "/v1/employees/99999";

        // Act
        var httpResponse = await _testClient.GetAsync(endpoint);
        var errorResponse = await httpResponse.Content.ReadFromJsonAsync<ErrorResponseDto>();

        // Assert
        Assert.NotNull(errorResponse);
        Assert.Equal("ORG-NTF-001", errorResponse.Code);
        Assert.False(string.IsNullOrWhiteSpace(errorResponse.Message));
    }

    #endregion
}
