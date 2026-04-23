using Api.Domain;
using Api.DTOs.Common;
using Api.DTOs.Employees;
using Api.Repositories;

namespace Api.Services;

/// <summary>
/// Implementation of <see cref="IEmployeeService"/>.
/// Maps domain entities to DTOs and wraps results in the standard envelope.
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeService"/> class.
    /// </summary>
    /// <param name="employeeRepository">Underlying repository.</param>
    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    /// <inheritdoc />
    public async Task<CollectionResponseDto<EmployeeDto>> GetAllAsync(
        EmployeeQuery query,
        string selfUrl,
        CancellationToken cancellationToken)
    {
        var employeeEntities = await _employeeRepository.GetAllAsync(query, cancellationToken);
        var totalMatchingCount = await _employeeRepository.CountAsync(query, cancellationToken);

        var employeeDtos = employeeEntities.Select(MapToDto).ToList();

        return new CollectionResponseDto<EmployeeDto>
        {
            Items = employeeDtos,
            Metadata = new MetadataDto
            {
                Timestamp = DateTime.UtcNow,
                TransactionId = Guid.NewGuid().ToString(),
                TotalCount = totalMatchingCount
            },
            Links = new LinksDto
            {
                Self = selfUrl,
                Next = null,
                Prev = null
            }
        };
    }

    /// <inheritdoc />
    public async Task<ItemResponseDto<EmployeeDto>?> GetByIdAsync(
        int employeeId,
        string selfUrl,
        CancellationToken cancellationToken)
    {
        var employeeEntity = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken);
        if (employeeEntity is null)
        {
            return null;
        }

        return new ItemResponseDto<EmployeeDto>
        {
            Item = MapToDto(employeeEntity),
            Metadata = new MetadataDto
            {
                Timestamp = DateTime.UtcNow,
                TransactionId = Guid.NewGuid().ToString(),
                TotalCount = null
            },
            Links = new LinksDto
            {
                Self = selfUrl,
                Next = null,
                Prev = null
            }
        };
    }

    private static EmployeeDto MapToDto(Employee employeeEntity)
    {
        return new EmployeeDto
        {
            EmployeeId = employeeEntity.EmployeeId,
            Name = employeeEntity.Name,
            Email = employeeEntity.Email,
            Department = employeeEntity.Department,
            HireDate = employeeEntity.HireDate,
            ActiveIndicator = employeeEntity.ActiveIndicator,
            Salary = employeeEntity.Salary,
            CreatedDate = employeeEntity.CreatedDate,
            ModifiedDate = employeeEntity.ModifiedDate
        };
    }
}
