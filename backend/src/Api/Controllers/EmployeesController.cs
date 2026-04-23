using Api.DTOs.Common;
using Api.DTOs.Employees;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// REST API controller for employee directory operations.
/// Provides filtered, paginated access to the employee dataset.
/// </summary>
[Route("v1/[controller]")]
[ApiController]
[Produces("application/json")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    /// <summary>
    /// Initializes a new instance of the EmployeesController.
    /// </summary>
    /// <param name="service">The employee business logic service.</param>
    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves a paginated, optionally filtered list of employees.
    /// Supports filtering by department and active status via query parameters.
    /// </summary>
    /// <param name="query">Query parameters controlling filtering and pagination.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of employees wrapped in the standard envelope.</returns>
    /// <response code="200">Employees returned successfully.</response>
    /// <response code="400">Invalid query parameters supplied.</response>
    /// <response code="500">An unexpected internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(CollectionResponseDto<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CollectionResponseDto<EmployeeDto>>> GetAllAsync(
        [FromQuery] EmployeeQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single employee record by their unique identifier.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The matching employee wrapped in the standard envelope, or 404 if not found.</returns>
    /// <response code="200">Employee found and returned successfully.</response>
    /// <response code="400">Invalid employee identifier supplied.</response>
    /// <response code="404">No employee exists with the specified identifier.</response>
    /// <response code="500">An unexpected internal server error occurred.</response>
    [HttpGet("{employeeId}")]
    [ProducesResponseType(typeof(ItemResponseDto<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ItemResponseDto<EmployeeDto>>> GetByIdAsync(
        int employeeId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(employeeId, cancellationToken);

        if (result is null)
        {
            return NotFound(new ErrorResponseDto
            {
                Code = "ORG-NTF-001",
                Message = $"Employee with identifier {employeeId} was not found."
            });
        }

        return Ok(result);
    }
}
