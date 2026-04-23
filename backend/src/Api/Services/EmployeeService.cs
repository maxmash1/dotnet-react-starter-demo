using Api.Domain;
using Api.DTOs.Common;
using Api.DTOs.Employees;
using Api.Repositories;

namespace Api.Services;

/// <summary>
/// Service implementation for employee business logic.
/// Coordinates repository calls, maps entities to DTOs, and wraps results in envelope pattern.
/// </summary>
public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;

    /// <summary>
    /// Initializes a new instance of EmployeeService.
    /// </summary>
    /// <param name="repository">The employee data access repository.</param>
    public EmployeeService(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<CollectionResponseDto<EmployeeDto>> GetAllAsync(
        EmployeeQuery query,
        CancellationToken cancellationToken)
    {
        var employees = await _repository.GetAllAsync(query, cancellationToken);
        var totalCount = await _repository.CountAsync(query, cancellationToken);

        var dtos = employees.Select(MapToDto);

        var selfLink = BuildCollectionLink(query.Offset, query.Limit, query.Department);

        var nextOffset = query.Offset + query.Limit;
        var nextLink = nextOffset < totalCount
            ? BuildCollectionLink(nextOffset, query.Limit, query.Department)
            : null;

        var prevOffset = query.Offset - query.Limit;
        var prevLink = query.Offset > 0
            ? BuildCollectionLink(Math.Max(0, prevOffset), query.Limit, query.Department)
            : null;

        return new CollectionResponseDto<EmployeeDto>
        {
            Items = dtos,
            Metadata = new MetadataDto
            {
                Timestamp = DateTime.UtcNow,
                TransactionId = Guid.NewGuid().ToString("D"),
                TotalCount = totalCount
            },
            Links = new LinksDto
            {
                Self = selfLink,
                Next = nextLink,
                Prev = prevLink
            }
        };
    }

    /// <inheritdoc />
    public async Task<ItemResponseDto<EmployeeDto>?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var employee = await _repository.GetByIdAsync(id, cancellationToken);
        if (employee is null)
            return null;

        return new ItemResponseDto<EmployeeDto>
        {
            Item = MapToDto(employee),
            Metadata = new MetadataDto
            {
                Timestamp = DateTime.UtcNow,
                TransactionId = Guid.NewGuid().ToString("D"),
                TotalCount = null
            },
            Links = new LinksDto
            {
                Self = $"/v1/employees/{id}",
                Next = null,
                Prev = null
            }
        };
    }

    // ---------------------------------------------------------------------------
    // Private helpers
    // ---------------------------------------------------------------------------

    private static EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            EmployeeId = employee.Id,
            FullName = employee.FullName,
            Email = employee.Email,
            Department = employee.Department,
            JobTitle = employee.JobTitle,
            HireDate = employee.HireDate,
            ActiveIndicator = employee.ActiveIndicator
        };
    }

    /// <summary>
    /// Builds a paginated collection URL path including optional department filter.
    /// </summary>
    private static string BuildCollectionLink(int offset, int limit, string? department)
    {
        var link = $"/v1/employees?offset={offset}&limit={limit}";

        if (!string.IsNullOrWhiteSpace(department))
            link += $"&department={Uri.EscapeDataString(department)}";

        return link;
    }
}
