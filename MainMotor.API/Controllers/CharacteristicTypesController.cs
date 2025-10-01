using MainMotor.API.Models;
using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MainMotor.API.Controllers;

/// <summary>
/// Controller for managing characteristic types
/// </summary>
/// <remarks>
/// Provides endpoints for listing characteristic types.
/// Characteristic types define categories like Color, Fuel Type, Transmission, etc.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CharacteristicTypesController : ControllerBase
{
    private readonly ICharacteristicTypeService _characteristicTypeService;

    public CharacteristicTypesController(ICharacteristicTypeService characteristicTypeService)
    {
        _characteristicTypeService = characteristicTypeService;
    }

    /// <summary>
    /// Get all active characteristic types
    /// </summary>
    /// <returns>List of all active characteristic types</returns>
    /// <response code="200">Returns the list of active characteristic types</response>
    /// <response code="500">Internal server error occurred</response>
    /// <example>
    /// GET /api/characteristictypes
    /// 
    /// Returns all active characteristic types (Color, Fuel Type, etc.)
    /// </example>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CharacteristicTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CharacteristicTypeDto>>> GetAll()
    {
        var characteristicTypes = await _characteristicTypeService.GetAllAsync();
        return Ok(characteristicTypes);
    }
}