namespace Api.Tests.Utils;

/// <summary>
/// Factory methods for creating test data objects.
/// Provides consistent, configurable test fixtures with sensible defaults.
/// </summary>
public static class TestDataBuilders
{
    /// <summary>
    /// Creates a sample health response for testing purposes.
    /// </summary>
    /// <param name="status">Health status value (default: "healthy").</param>
    /// <param name="version">Application version (default: "1.0.0.0").</param>
    /// <param name="environment">Runtime environment (default: "Testing").</param>
    /// <returns>A configured health response DTO.</returns>
    public static Api.DTOs.Health.HealthResponseDto BuildHealthResponse(
        string status = "healthy",
        string version = "1.0.0.0",
        string environment = "Testing")
    {
        return new Api.DTOs.Health.HealthResponseDto
        {
            Status = status,
            Version = version,
            CheckedAtDate = DateTime.UtcNow,
            Environment = environment
        };
    }

    /// <summary>
    /// Creates sample metadata for envelope responses.
    /// </summary>
    /// <param name="transactionId">Correlation identifier (default: new GUID).</param>
    /// <param name="totalCount">Optional total count for collections.</param>
    /// <returns>A configured metadata DTO.</returns>
    public static Api.DTOs.Common.MetadataDto BuildMetadata(
        string? transactionId = null,
        int? totalCount = null)
    {
        return new Api.DTOs.Common.MetadataDto
        {
            Timestamp = DateTime.UtcNow,
            TransactionId = transactionId ?? Guid.NewGuid().ToString("D"),
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Creates sample links for envelope responses.
    /// </summary>
    /// <param name="selfUrl">Canonical URL of the resource.</param>
    /// <param name="nextUrl">Optional next page URL.</param>
    /// <param name="prevUrl">Optional previous page URL.</param>
    /// <returns>A configured links DTO.</returns>
    public static Api.DTOs.Common.LinksDto BuildLinks(
        string selfUrl = "http://localhost/v1/resource",
        string? nextUrl = null,
        string? prevUrl = null)
    {
        return new Api.DTOs.Common.LinksDto
        {
            Self = selfUrl,
            Next = nextUrl,
            Prev = prevUrl
        };
    }

    /// <summary>
    /// Creates a sample Employee domain entity for testing purposes.
    /// </summary>
    /// <param name="id">Employee primary key (default: 1).</param>
    /// <param name="fullName">Full display name (default: "Test Employee").</param>
    /// <param name="email">Email address (default: "test.employee@example.com").</param>
    /// <param name="department">Department name (default: "Engineering").</param>
    /// <param name="jobTitle">Job title (default: "Software Engineer").</param>
    /// <param name="active">Active status indicator (default: true).</param>
    /// <returns>A configured Employee domain entity.</returns>
    public static Api.Domain.Employee BuildEmployee(
        int id = 1,
        string fullName = "Test Employee",
        string email = "test.employee@example.com",
        string department = "Engineering",
        string jobTitle = "Software Engineer",
        bool active = true)
    {
        return new Api.Domain.Employee
        {
            Id = id,
            FullName = fullName,
            Email = email,
            Department = department,
            JobTitle = jobTitle,
            HireDate = new DateTime(2022, 1, 1),
            ActiveIndicator = active
        };
    }
}
