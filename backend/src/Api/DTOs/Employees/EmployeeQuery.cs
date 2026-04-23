using System.Text.Json.Serialization;

namespace Api.DTOs.Employees;

/// <summary>
/// Query parameters for filtering and paginating employee results.
/// </summary>
public sealed class EmployeeQuery
{
    /// <summary>
    /// Number of records to skip (default: 0).
    /// </summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; } = 0;

    /// <summary>
    /// Maximum number of records to return (default: 20, max: 100).
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; } = 20;

    /// <summary>
    /// Optional department code filter (empty string = no filter).
    /// </summary>
    [JsonPropertyName("department")]
    public string? Department { get; set; }

    /// <summary>
    /// Optional active status filter (null = no filter).
    /// </summary>
    [JsonPropertyName("activeIndicator")]
    public bool? ActiveIndicator { get; set; }
}
