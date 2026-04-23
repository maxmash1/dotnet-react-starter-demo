namespace Api.Domain;

/// <summary>
/// Represents an employee domain entity.
/// </summary>
public class Employee
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Department { get; set; }
    public required string JobTitle { get; set; }
    public DateTime? HireDate { get; set; }
    public bool ActiveIndicator { get; set; }
}
