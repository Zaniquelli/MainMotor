using MainMotor.API.Models;
using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MainMotor.API.Controllers;

/// <summary>
/// Controller for managing vehicle characteristics
/// </summary>
/// <remarks>
/// Provides endpoints for listing characteristics and filtering by type.
/// Characteristics are used to define vehicle properties like color, fuel type, etc.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CharacteristicsController : ControllerBase
{
    private readonly ICharacteristicService _characteristicService;

    public CharacteristicsController(ICharacteristicService characteristicService)
    {
        _characteristicService = characteristicService;
    }

    /// <summary>
    /// Get all active characteristics
    /// </summary>
    /// <returns>List of all active characteristics with their types</returns>
    /// <response code="200">Returns the list of active characteristics</response>
    /// <response code="500">Internal server error occurred</response>
    /// <example>
    /// GET /api/characteristics
    /// 
    /// Returns all active characteristics grouped by type
    /// </example>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CharacteristicDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CharacteristicDto>>> GetAll()
    {
        var characteristics = await _characteristicService.GetAllAsync();
        return Ok(characteristics);
    }

    /// <summary>
    /// Get characteristics by type
    /// </summary>
    /// <param name="typeId">Unique identifier of the characteristic type (UUID format)</param>
    /// /// <returns>Licharacteristics belonging to the specified type</returns>
    /// <response code="200">Returns the list of characteristics for the specified type</response>
    /// <response code="400">Invalid characteristic type ID format</response>
    /// <response code="500">Internal server error occurred</response>
    /// <example>
    /// GET /api/characteristics/by-type/123e4567-e89b-12d3-a456-426614174000
    /// 
    /// Returns all characteristics of the specified type (e.g., all colors)
    /// </example>
    [HttpGet("by-type/{typeId}")]
    [ProducesResponseType(typeof(IEnumerable<CharacteristicDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CharacteristicDto>>> GetByType([Required] Guid typeId)
    {
        var characteristics = await _characteristicService.GetByTypeAsync(typeId);
        return Ok(characteristics);
    }
}