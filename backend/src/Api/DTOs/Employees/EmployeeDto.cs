using System.Text.Json.Serialization;

namespace Api.DTOs.Employees;

/// <summary>
/// Represents an employee response payload.
/// </summary>
public sealed class EmployeeDto
{
    /// <summary>
    /// The unique identifier of the employee.
    /// </summary>
    [JsonPropertyName("employeeId")]
    public int EmployeeId { get; set; }

    /// <summary>
    /// The full name of the employee.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// The email address of the employee.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// The department code the employee belongs to.
    /// </summary>
    [JsonPropertyName("department")]
    public string? Department { get; set; }

    /// <summary>
    /// The date the employee was hired.
    /// </summary>
    [JsonPropertyName("hireDate")]
    public DateTime? HireDate { get; set; }

    /// <summary>
    /// Indicates whether the employee is currently active.
    /// </summary>
    [JsonPropertyName("activeIndicator")]
    public bool ActiveIndicator { get; set; }

    /// <summary>
    /// The annual salary of the employee.
    /// </summary>
    [JsonPropertyName("salary")]
    public decimal? Salary { get; set; }

    /// <summary>
    /// The date the employee record was created.
    /// </summary>
    [JsonPropertyName("createdDate")]
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// The date the employee record was last modified.
    /// </summary>
    [JsonPropertyName("modifiedDate")]
    public DateTime ModifiedDate { get; set; }
}
