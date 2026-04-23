using System.Text.Json.Serialization;

namespace Api.DTOs.Employees;

/// <summary>
/// Query parameters for filtering and paginating employee results.
/// All filter fields are optional; omitting them returns the full unfiltered set.
/// </summary>
public sealed class EmployeeQuery
{
    /// <summary>
    /// Filters employees by exact department name (e.g. "Engineering", "HR").
    /// Null or empty string returns employees from all departments.
    /// </summary>
    [JsonPropertyName("department")]
    public string? Department { get; set; }

    /// <summary>
    /// When true, only employees with ActiveIndicator = true are returned.
    /// When false (default), all employees regardless of active status are returned.
    /// </summary>
    [JsonPropertyName("activeOnlyIndicator")]
    public bool ActiveOnlyIndicator { get; set; } = false;

    /// <summary>
    /// Number of records to skip before returning results. Default is 0.
    /// </summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; } = 0;

    /// <summary>
    /// Maximum number of records to return per page. Default is 20, maximum is 100.
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; } = 20;
}
