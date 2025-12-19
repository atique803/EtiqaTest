using Microsoft.AspNetCore.Mvc;
using PayrollSystem.Application.DTOs;
using PayrollSystem.Application.Interfaces;

namespace PayrollSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    /// Get all employees
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll([FromQuery] bool includeArchived = false)
    {
        try
        {
            var employees = await _employeeService.GetAllEmployeesAsync(includeArchived);
            return Ok(employees);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving employees", error = ex.Message });
        }
    }

    /// <summary>
    /// Get employee by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int id)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound(new { message = "Employee not found" });

            return Ok(employee);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving employee", error = ex.Message });
        }
    }

    /// <summary>
    /// Get employee by employee number
    /// </summary>
    [HttpGet("by-number/{employeeNumber}")]
    public async Task<ActionResult<EmployeeDto>> GetByNumber(string employeeNumber)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByNumberAsync(employeeNumber);
            if (employee == null)
                return NotFound(new { message = "Employee not found" });

            return Ok(employee);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving employee", error = ex.Message });
        }
    }

    /// <summary>
    /// Search employees by employee number or name (wildcard search)
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> Search(
        [FromQuery] string searchTerm, 
        [FromQuery] bool includeArchived = false)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest(new { message = "Search term is required" });

            var employees = await _employeeService.SearchEmployeesAsync(searchTerm, includeArchived);
            return Ok(employees);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error searching employees", error = ex.Message });
        }
    }

    /// <summary>
    /// Create a new employee
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create([FromBody] CreateEmployeeDto dto)
    {
        try
        {
            var employee = await _employeeService.CreateEmployeeAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating employee", error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing employee
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<EmployeeDto>> Update(int id, [FromBody] UpdateEmployeeDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(new { message = "ID mismatch" });

            var employee = await _employeeService.UpdateEmployeeAsync(dto);
            return Ok(employee);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("not found"))
                return NotFound(new { message = ex.Message });
            
            return StatusCode(500, new { message = "Error updating employee", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete an employee
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);
            if (!result)
                return NotFound(new { message = "Employee not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting employee", error = ex.Message });
        }
    }

    /// <summary>
    /// Archive an employee
    /// </summary>
    [HttpPost("{id}/archive")]
    public async Task<ActionResult> Archive(int id)
    {
        try
        {
            var result = await _employeeService.ArchiveEmployeeAsync(id);
            if (!result)
                return NotFound(new { message = "Employee not found" });

            return Ok(new { message = "Employee archived successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error archiving employee", error = ex.Message });
        }
    }

    /// <summary>
    /// Unarchive an employee
    /// </summary>
    [HttpPost("{id}/unarchive")]
    public async Task<ActionResult> Unarchive(int id)
    {
        try
        {
            var result = await _employeeService.UnarchiveEmployeeAsync(id);
            if (!result)
                return NotFound(new { message = "Employee not found" });

            return Ok(new { message = "Employee unarchived successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error unarchiving employee", error = ex.Message });
        }
    }

    /// <summary>
    /// Calculate employee salary for a date range
    /// </summary>
    [HttpPost("calculate-salary")]
    public async Task<ActionResult<CalculateSalaryResponse>> CalculateSalary([FromBody] CalculateSalaryRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.EmployeeNumber))
                return BadRequest(new { message = "Employee number is required" });

            if (request.StartDate > request.EndDate)
                return BadRequest(new { message = "Start date must be before or equal to end date" });

            var result = await _employeeService.CalculateSalaryAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("not found"))
                return NotFound(new { message = ex.Message });
            
            return StatusCode(500, new { message = "Error calculating salary", error = ex.Message });
        }
    }
}
