namespace Api.Tests.Utils;

/// <summary>
/// Factory methods for creating test data objects.
/// Provides consistent, configurable test fixtures with sensible defaults.
/// </summary>
public static class TestDataBuilders
{
    /// <summary>
    /// Wraps a payload in a single-item response envelope using the provided
    /// metadata and links, or sensible defaults when omitted.
    /// </summary>
    /// <typeparam name="T">The type of the item payload.</typeparam>
    /// <param name="item">The resource payload to wrap.</param>
    /// <param name="metadata">Optional metadata; a new default instance is used when null.</param>
    /// <param name="links">Optional links; a new default instance is used when null.</param>
    /// <returns>A fully populated <see cref="Api.DTOs.Common.ItemResponseDto{T}"/>.</returns>
    public static Api.DTOs.Common.ItemResponseDto<T> BuildItemResponse<T>(
        T item,
        Api.DTOs.Common.MetadataDto? metadata = null,
        Api.DTOs.Common.LinksDto? links = null)
    {
        return new Api.DTOs.Common.ItemResponseDto<T>
        {
            Item = item,
            Metadata = metadata ?? BuildMetadata(),
            Links = links ?? BuildLinks()
        };
    }

    /// <summary>
    /// Wraps a collection of items in a collection response envelope using the provided
    /// metadata and links, or sensible defaults when omitted.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="items">The resource collection to wrap.</param>
    /// <param name="metadata">Optional metadata; a new default instance is used when null.</param>
    /// <param name="links">Optional links; a new default instance is used when null.</param>
    /// <returns>A fully populated <see cref="Api.DTOs.Common.CollectionResponseDto{T}"/>.</returns>
    public static Api.DTOs.Common.CollectionResponseDto<T> BuildCollectionResponse<T>(
        IEnumerable<T> items,
        Api.DTOs.Common.MetadataDto? metadata = null,
        Api.DTOs.Common.LinksDto? links = null)
    {
        return new Api.DTOs.Common.CollectionResponseDto<T>
        {
            Items = items,
            Metadata = metadata ?? BuildMetadata(),
            Links = links ?? BuildLinks()
        };
    }

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
}
