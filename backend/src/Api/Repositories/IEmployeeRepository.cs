using Api.Domain;
using Api.DTOs.Employees;

namespace Api.Repositories;

/// <summary>
/// Repository interface for employee data access operations.
/// All read operations use AsNoTracking for optimal performance.
/// </summary>
public interface IEmployeeRepository
{
    /// <summary>
    /// Retrieves a paged, filtered list of employees.
    /// </summary>
    /// <param name="query">Filter and pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of employee entities matching the query criteria.</returns>
    Task<IEnumerable<Employee>> GetAllAsync(EmployeeQuery query, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the total count of employees matching the query filters, ignoring pagination.
    /// Used to build HATEOAS pagination links and surface totalCount in metadata.
    /// </summary>
    /// <param name="query">Filter parameters (Offset and Limit are ignored).</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The total number of employees matching the filter criteria.</returns>
    Task<int> CountAsync(EmployeeQuery query, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a single employee by their unique database identifier.
    /// </summary>
    /// <param name="id">The primary key of the employee record.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The matching employee entity, or null if no record was found.</returns>
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken);
}
