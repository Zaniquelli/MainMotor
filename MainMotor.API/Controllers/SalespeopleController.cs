using Microsoft.AspNetCore.Mvc;
using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;

namespace MainMotor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalespeopleController : ControllerBase
{
    private readonly ISalespersonService _salespersonService;

    public SalespeopleController(ISalespersonService salespersonService)
    {
        _salespersonService = salespersonService;
    }

    /// <summary>
    /// Get all salespeople
    /// </summary>
    /// <returns>List of salespeople</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SalespersonDto>>> GetAll()
    {
        var salespeople = await _salespersonService.GetAllAsync();
        return Ok(salespeople);
    }

    /// <summary>
    /// Get salesperson by ID
    /// </summary>
    /// <param name="id">Salesperson ID</param>
    /// <returns>Salesperson details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<SalespersonDto>> GetById(Guid id)
    {
        var salesperson = await _salespersonService.GetByIdAsync(id);
        if (salesperson == null)
        {
            return NotFound();
        }
        return Ok(salesperson);
    }

    /// <summary>
    /// Create a new salesperson
    /// </summary>
    /// <param name="createSalespersonDto">Salesperson data</param>
    /// <returns>Created salesperson</returns>
    [HttpPost]
    public async Task<ActionResult<SalespersonDto>> Create(CreateSalespersonDto createSalespersonDto)
    {
        var salesperson = await _salespersonService.CreateAsync(createSalespersonDto);
        return CreatedAtAction(nameof(GetById), new { id = salesperson.Id }, salesperson);
    }

    /// <summary>
    /// Update an existing salesperson
    /// </summary>
    /// <param name="id">Salesperson ID</param>
    /// <param name="updateSalespersonDto">Updated salesperson data</param>
    /// <returns>Updated salesperson</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<SalespersonDto>> Update(Guid id, UpdateSalespersonDto updateSalespersonDto)
    {
        var salesperson = await _salespersonService.UpdateAsync(id, updateSalespersonDto);
        if (salesperson == null)
        {
            return NotFound();
        }
        return Ok(salesperson);
    }

    /// <summary>
    /// Delete a salesperson
    /// </summary>
    /// <param name="id">Salesperson ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _salespersonService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}