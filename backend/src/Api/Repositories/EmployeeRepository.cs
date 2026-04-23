using Api.Data;
using Api.Domain;
using Api.DTOs.Employees;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

/// <summary>
/// EF Core implementation of the employee repository.
/// Uses AsNoTracking on all read operations and LINQ for all data access.
/// </summary>
public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of EmployeeRepository.
    /// </summary>
    /// <param name="context">The EF Core database context.</param>
    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Employee>> GetAllAsync(
        EmployeeQuery query,
        CancellationToken cancellationToken)
    {
        var queryable = _context.Employees.AsNoTracking();

        // Apply department filter — case-insensitive; empty/null string means no filter
        if (!string.IsNullOrWhiteSpace(query.Department))
            queryable = queryable.Where(e => e.Department.ToLower() == query.Department.ToLower());

        // Apply active-only filter
        if (query.ActiveOnlyIndicator)
            queryable = queryable.Where(e => e.ActiveIndicator);

        return await queryable
            .OrderBy(e => e.FullName)
            .Skip(query.Offset)
            .Take(query.Limit)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(
        EmployeeQuery query,
        CancellationToken cancellationToken)
    {
        var queryable = _context.Employees.AsNoTracking();

        // Apply the same filters as GetAllAsync, but without pagination
        if (!string.IsNullOrWhiteSpace(query.Department))
            queryable = queryable.Where(e => e.Department.ToLower() == query.Department.ToLower());

        if (query.ActiveOnlyIndicator)
            queryable = queryable.Where(e => e.ActiveIndicator);

        return await queryable.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
}
