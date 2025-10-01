using MainMotor.API.Models;
using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MainMotor.API.Controllers;

/// <summary>
/// Controller for managing vehicle brands
/// </summary>
/// <remarks>
/// Provides endpoints for listing vehicle brands.
/// Brands represent vehicle manufacturers like Toyota, Ford, BMW, etc.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandsController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    /// <summary>
    /// Get all active brands
    /// </summary>
    /// <returns>List of all active vehicle brands</returns>
    /// <response code="200">Returns the list of active brands</response>
    /// <response code="500">Internal server error occurred</response>
    /// <example>
    /// GET /api/brands
    /// 
    /// Returns all active vehicle brands with their FIPE codes
    /// </example>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BrandDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<BrandDto>>> GetAll()
    {
        var brands = await _brandService.GetAllAsync();
        return Ok(brands);
    }
}