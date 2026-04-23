using System.Text.Json.Serialization;

namespace Api.DTOs.Employees;

/// <summary>
/// Response DTO representing a single employee resource.
/// Returned in both collection and single-item endpoint envelopes.
/// </summary>
public sealed class EmployeeDto
{
    /// <summary>
    /// The unique identifier of the employee.
    /// </summary>
    [JsonPropertyName("employeeId")]
    public int EmployeeId { get; set; }

    /// <summary>
    /// The full display name of the employee.
    /// </summary>
    [JsonPropertyName("fullName")]
    public required string FullName { get; set; }

    /// <summary>
    /// The corporate email address of the employee.
    /// </summary>
    [JsonPropertyName("email")]
    public required string Email { get; set; }

    /// <summary>
    /// The department the employee belongs to (e.g. Engineering, Product, HR).
    /// </summary>
    [JsonPropertyName("department")]
    public required string Department { get; set; }

    /// <summary>
    /// The job title of the employee within their department.
    /// </summary>
    [JsonPropertyName("jobTitle")]
    public required string JobTitle { get; set; }

    /// <summary>
    /// The date the employee was hired. Null if not yet on record.
    /// </summary>
    [JsonPropertyName("hireDate")]
    public DateTime? HireDate { get; set; }

    /// <summary>
    /// Indicates whether the employee is currently active in the organization.
    /// </summary>
    [JsonPropertyName("activeIndicator")]
    public bool ActiveIndicator { get; set; }
}
