using Api.Data;
using Api.Domain;
using Api.DTOs.Employees;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IEmployeeRepository"/>.
/// </summary>
public class EmployeeRepository : IEmployeeRepository
{
    private const int MaximumPageSize = 100;

    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeRepository"/> class.
    /// </summary>
    /// <param name="dbContext">EF Core database context.</param>
    public EmployeeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Employee>> GetAllAsync(
        EmployeeQuery query,
        CancellationToken cancellationToken)
    {
        var employeeQueryable = BuildFilteredQueryable(query);

        var pageOffset = query.Offset < 0 ? 0 : query.Offset;
        var pageLimit = query.Limit <= 0
            ? 20
            : Math.Min(query.Limit, MaximumPageSize);

        return await employeeQueryable
            .OrderBy(employee => employee.Name)
            .Skip(pageOffset)
            .Take(pageLimit)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(EmployeeQuery query, CancellationToken cancellationToken)
    {
        return await BuildFilteredQueryable(query).CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Employee?> GetByIdAsync(int employeeId, CancellationToken cancellationToken)
    {
        return await _dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(employee => employee.EmployeeId == employeeId, cancellationToken);
    }

    private IQueryable<Employee> BuildFilteredQueryable(EmployeeQuery query)
    {
        var employeeQueryable = _dbContext.Employees.AsNoTracking();

        // Empty/null department string means "no filter"
        if (!string.IsNullOrWhiteSpace(query.Department))
        {
            var departmentFilter = query.Department;
            employeeQueryable = employeeQueryable.Where(employee => employee.Department == departmentFilter);
        }

        // Null active indicator means "no filter"
        if (query.ActiveIndicator.HasValue)
        {
            var activeFilter = query.ActiveIndicator.Value;
            employeeQueryable = employeeQueryable.Where(employee => employee.ActiveIndicator == activeFilter);
        }

        return employeeQueryable;
    }
}
