using Api.Domain;
using Api.DTOs.Employees;

namespace Api.Repositories;

/// <summary>
/// Repository interface for employee data access.
/// </summary>
public interface IEmployeeRepository
{
    /// <summary>
    /// Gets all employees matching the specified query, with pagination applied.
    /// </summary>
    /// <param name="query">Filter and pagination parameters.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>Page of matching employees.</returns>
    Task<IEnumerable<Employee>> GetAllAsync(EmployeeQuery query, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the total count of employees matching the specified query (ignoring pagination).
    /// </summary>
    /// <param name="query">Filter parameters.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>Total matching record count.</returns>
    Task<int> CountAsync(EmployeeQuery query, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a single employee by its unique identifier.
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The employee, or null if not found.</returns>
    Task<Employee?> GetByIdAsync(int employeeId, CancellationToken cancellationToken);
}
