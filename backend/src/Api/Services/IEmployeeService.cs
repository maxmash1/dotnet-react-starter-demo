using Api.DTOs.Common;
using Api.DTOs.Employees;

namespace Api.Services;

/// <summary>
/// Service interface for employee business logic.
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Gets all employees matching the query, wrapped in a collection envelope.
    /// </summary>
    /// <param name="query">Filter and pagination parameters.</param>
    /// <param name="selfUrl">Canonical URL of the current request.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    Task<CollectionResponseDto<EmployeeDto>> GetAllAsync(
        EmployeeQuery query,
        string selfUrl,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a single employee by id, wrapped in an item envelope. Returns null when not found.
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee.</param>
    /// <param name="selfUrl">Canonical URL of the current request.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    Task<ItemResponseDto<EmployeeDto>?> GetByIdAsync(
        int employeeId,
        string selfUrl,
        CancellationToken cancellationToken);
}
