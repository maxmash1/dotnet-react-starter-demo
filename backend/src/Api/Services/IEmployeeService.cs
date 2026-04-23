using Api.DTOs.Common;
using Api.DTOs.Employees;

namespace Api.Services;

/// <summary>
/// Service interface for employee business logic.
/// Maps domain entities to response DTOs and wraps results in the envelope pattern.
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Retrieves a paged, filtered collection of employees wrapped in the standard envelope.
    /// </summary>
    /// <param name="query">Filter and pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>
    /// A <see cref="CollectionResponseDto{T}"/> containing matched employees,
    /// metadata with totalCount, and HATEOAS pagination links.
    /// </returns>
    Task<CollectionResponseDto<EmployeeDto>> GetAllAsync(
        EmployeeQuery query,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a single employee by unique identifier wrapped in the standard envelope.
    /// </summary>
    /// <param name="id">The primary key of the employee record.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>
    /// An <see cref="ItemResponseDto{T}"/> containing the employee,
    /// or null if no record was found.
    /// </returns>
    Task<ItemResponseDto<EmployeeDto>?> GetByIdAsync(int id, CancellationToken cancellationToken);
}
