namespace Api.Domain;

/// <summary>
/// Represents an employee entity persisted in the data store.
/// </summary>
public class Employee
{
    /// <summary>
    /// The unique identifier of the employee.
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// The full name of the employee.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The email address of the employee.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// The department code the employee belongs to.
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// The date the employee was hired.
    /// </summary>
    public DateTime? HireDate { get; set; }

    /// <summary>
    /// Indicates whether the employee is currently active.
    /// </summary>
    public bool ActiveIndicator { get; set; }

    /// <summary>
    /// The annual salary of the employee.
    /// </summary>
    public decimal? Salary { get; set; }

    /// <summary>
    /// The date the employee record was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// The date the employee record was last modified.
    /// </summary>
    public DateTime ModifiedDate { get; set; }
}
