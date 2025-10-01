using MainMotor.API.Models;
using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MainMotor.API.Controllers;

/// <summary>
/// Controller for managing vehicle models
/// </summary>
/// <remarks>
/// Provides endpoints for listing vehicle models and filtering by brand.
/// Models represent specific vehicle models like Corolla, F-150, X3, etc.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ModelsController : ControllerBase
{
    private readonly IModelService _modelService;

    public ModelsController(IModelService modelService)
    {
        _modelService = modelService;
    }

    /// <summary>
    /// Get all active models
    /// </summary>
    /// <returns>List of all active vehicle models with brand information</returns>
    /// <response code="200">Returns the list of active models</response>
    /// <response code="500">Internal server error occurred</response>
    /// <example>
    /// GET /api/models
    /// 
    /// Returns all active vehicle models with their brand names and FIPE codes
    /// </example>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ModelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ModelDto>>> GetAll()
    {
        var models = await _modelService.GetAllAsync();
        return Ok(models);
    }

    /// <summary>
    /// Get models by brand
    /// </summary>
    /// <param name="brandId">Unique identifier of the brand (UUID format)</param>
    /// <returns>List of models belonging to the specified brand</returns>
    /// <response code="200">Returns the list of models for the specified brand</response>
    /// <response code="400">Invalid brand ID format</response>
    /// <response code="500">Internal server error occurred</response>
    /// <example>
    /// GET /api/models/by-brand/123e4567-e89b-12d3-a456-426614174000
    /// 
    /// Returns all models for the specified brand (e.g., all Toyota models)
    /// </example>
    [HttpGet("by-brand/{brandId}")]
    [ProducesResponseType(typeof(IEnumerable<ModelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ModelDto>>> GetByBrand([Required] Guid brandId)
    {
        var models = await _modelService.GetByBrandAsync(brandId);
        return Ok(models);
    }
}