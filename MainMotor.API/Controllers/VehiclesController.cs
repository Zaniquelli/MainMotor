using Microsoft.AspNetCore.Mvc;
using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;
using MainMotor.Domain.Enums;
using MainMotor.API.Models;
using System.ComponentModel.DataAnnotations;

namespace MainMotor.API.Controllers;

/// <summary>
/// Controller for managing vehicles in the MainMotor marketplace
/// </summary>
/// <remarks>
/// Provides endpoints for vehicle registration, editing, listing, and status management.
/// Vehicles can be filtered by status and ordered by price for marketplace functionality.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    /// <summary>
    /// Get vehicles with optional filtering and ordering
    /// </summary>
    /// <param name="status">Filter by vehicle status. Available values: 1=Available, 2=Reserved, 3=Sold, 4=InMaintenance, 5=Unavailable</param>
    /// <param name="orderBy">Order by price (use 'price' to order by price ascending, leave empty for default ordering)</param>
    /// <returns>List of vehicles matching the specified criteria</returns>
    /// <response code="200">Returns the list of vehicles. Includes brand name, model name, year, and sale date for sold vehicles.</response>
    /// <response code="400">Invalid status parameter or orderBy parameter</response>
    /// <response code="500">Internal server error occurred</response>
    /// <example>
    /// GET /api/vehicles?status=1&amp;orderBy=price
    /// 
    /// Returns all available vehicles ordered by price (lowest to highest)
    /// </example>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAll([FromQuery] VehicleStatus? status = null, [FromQuery] string? orderBy = null)
    {
        bool orderByPrice = !string.IsNullOrEmpty(orderBy) && orderBy.ToLower() == "price";
        var vehicles = await _vehicleService.GetVehiclesByStatusAsync(status, orderByPrice);
        return Ok(vehicles);
    }

    /// <summary>
    /// Get vehicle by ID
    /// </summary>
    /// <param name="id">Unique identifier of the vehicle (UUID format)</param>
    /// <returns>Vehicle details including brand, model, year, and current status</returns>
    /// <response code="200">Returns the vehicle details</response>
    /// <response code="404">Vehicle not found with the specified ID</response>
    /// <response code="400">Invalid vehicle ID format</response>
    /// <response code="500">Internal server error occurred</response>
    /// <example>
    /// GET /api/vehicles/123e4567-e89b-12d3-a456-426614174000
    /// </example>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VehicleDto>> GetById([Required] Guid id)
    {
        var vehicle = await _vehicleService.GetByIdAsync(id);
        if (vehicle == null)
        {
            return NotFound();
        }
        return Ok(vehicle);
    }

    /// <summary>
    /// Create a new vehicle for sale
    /// </summary>
    /// <param name="createVehicleDto">Vehicle registration data including VIN, license plate, characteristics, mileage, prices, and model year</param>
    /// <returns>Created vehicle with generated ID and default status (Available)</returns>
    /// <response code="201">Vehicle created successfully. Returns the created vehicle with its assigned ID.</response>
    /// <response code="400">Invalid vehicle data. Check validation errors for specific field requirements.</response>
    /// <response code="409">Vehicle with the same VIN or license plate already exists</response>
    /// <response code="500">Internal server error occurred</response>
    /// <remarks>
    /// Required fields:
    /// - VinNumber: Must be exactly 17 characters, valid VIN format (no I, O, or Q)
    /// - LicensePlate: 7-10 characters
    /// - CharacteristicIds: At least one characteristic must be selected (e.g., color, transmission type)
    /// - Mileage: 0-999,999 km
    /// - PurchasePrice: Must be greater than 0
    /// - SalePrice: Must be greater than 0
    /// - ModelYearId: Must reference an existing model year
    /// 
    /// Optional fields:
    /// - Notes: Up to 500 characters
    /// - Status: Defaults to Available (1)
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VehicleDto>> Create([FromBody, Required] CreateVehicleDto createVehicleDto)
    {
        var vehicle = await _vehicleService.CreateAsync(createVehicleDto);
        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
    }

    /// <summary>
    /// Update an existing vehicle (only allowed if status is Available)
    /// </summary>
    /// <param name="id">Unique identifier of the vehicle to update</param>
    /// <param name="updateVehicleDto">Updated vehicle data with the same validation rules as creation</param>
    /// <returns>Updated vehicle with modified information</returns>
    /// <response code="200">Vehicle updated successfully</response>
    /// <response code="400">Invalid vehicle data, vehicle cannot be edited (not Available status), or characteristic validation failures</response>
    /// <response code="404">Vehicle not found with the specified ID, or one or more characteristics not found</response>
    /// <response code="409">Updated VIN or license plate conflicts with existing vehicle</response>
    /// <response code="500">Internal server error occurred</response>
    /// <remarks>
    /// Business Rules:
    /// - Only vehicles with status "Available" (1) can be edited
    /// - Vehicles that are Reserved (2), Sold (3), InMaintenance (4), or Unavailable (5) cannot be modified
    /// - All validation rules from vehicle creation apply to updates
    /// - VIN and license plate must remain unique across all vehicles
    /// - All characteristics must exist and be active
    /// - At least one characteristic must be selected
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VehicleDto>> Update([Required] Guid id, [FromBody, Required] UpdateVehicleDto updateVehicleDto)
    {
        var vehicle = await _vehicleService.UpdateAsync(id, updateVehicleDto);
        return Ok(vehicle);
    }

    /// <summary>
    /// Delete a vehicle from the system
    /// </summary>
    /// <param name="id">Unique identifier of the vehicle to delete</param>
    /// <returns>No content if deletion is successful</returns>
    /// <response code="204">Vehicle deleted successfully</response>
    /// <response code="400">Cannot delete vehicle (may have associated sales or payments)</response>
    /// <response code="404">Vehicle not found with the specified ID</response>
    /// <response code="500">Internal server error occurred</response>
    /// <remarks>
    /// Business Rules:
    /// - Vehicles with associated sales cannot be deleted
    /// - Consider setting status to Unavailable instead of deletion for audit purposes
    /// </remarks>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([Required] Guid id)
    {
        await _vehicleService.DeleteAsync(id);
        return NoContent();
    }
}