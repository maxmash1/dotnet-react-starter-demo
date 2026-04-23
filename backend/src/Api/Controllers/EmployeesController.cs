using Api.DTOs.Common;
using Api.DTOs.Employees;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// API controller for employee operations.
/// </summary>
[Route("v1/[controller]")]
[ApiController]
[Produces("application/json")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeesController"/> class.
    /// </summary>
    /// <param name="employeeService">Service handling employee business logic.</param>
    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    /// Retrieves all employees with optional filtering and pagination.
    /// </summary>
    /// <param name="query">Filter and pagination parameters.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A collection of employees wrapped in the standard envelope.</returns>
    /// <response code="200">Collection returned successfully.</response>
    /// <response code="400">Invalid request parameters.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [ProducesResponseType(typeof(CollectionResponseDto<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CollectionResponseDto<EmployeeDto>>> GetAllAsync(
        [FromQuery] EmployeeQuery query,
        CancellationToken cancellationToken)
    {
        var selfUrl = BuildSelfUrl();
        var collectionEnvelope = await _employeeService.GetAllAsync(query, selfUrl, cancellationToken);
        return Ok(collectionEnvelope);
    }

    /// <summary>
    /// Retrieves a single employee by its unique identifier.
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The requested employee wrapped in the standard envelope.</returns>
    /// <response code="200">Employee found successfully.</response>
    /// <response code="404">Employee not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("{employeeId:int}")]
    [ProducesResponseType(typeof(ItemResponseDto<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ItemResponseDto<EmployeeDto>>> GetByIdAsync(
        int employeeId,
        CancellationToken cancellationToken)
    {
        var selfUrl = BuildSelfUrl();
        var itemEnvelope = await _employeeService.GetByIdAsync(employeeId, selfUrl, cancellationToken);
        if (itemEnvelope is null)
        {
            return NotFound(new ErrorResponseDto
            {
                Code = "ORG-NTF-001",
                Message = $"Employee with id '{employeeId}' was not found."
            });
        }

        return Ok(itemEnvelope);
    }

    private string BuildSelfUrl()
    {
        var queryString = Request.QueryString.HasValue ? Request.QueryString.Value : string.Empty;
        return $"{Request.Scheme}://{Request.Host}{Request.PathBase}{Request.Path}{queryString}";
    }
}
