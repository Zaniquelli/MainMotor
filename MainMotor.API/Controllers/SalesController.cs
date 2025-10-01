using Microsoft.AspNetCore.Mvc;
using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;
using MainMotor.API.Models;
using System.ComponentModel.DataAnnotations;

namespace MainMotor.API.Controllers;

/// <summary>
/// Controller for managing vehicle sales and transactions
/// </summary>
/// <remarks>
/// Handles the complete sales process including customer registration, vehicle reservation,
/// and integration with payment systems. Supports both direct sales creation and 
/// marketplace-style sales registration with CPF validation.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    /// <summary>
    /// Get all sales in the system
    /// </summary>
    /// <returns>Complete list of sales with vehicle, customer, and salesperson information</returns>
    /// <response code="200">Returns the list of all sales</response>
    /// <response code="500">Internal server error occurred</response>
    /// <example>
    /// GET /api/sales
    /// 
    /// Returns all sales with complete transaction details
    /// </example>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SaleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SaleDto>>> GetAll()
    {
        var sales = await _saleService.GetAllAsync();
        return Ok(sales);
    }

    /// <summary>
    /// Get sale by ID
    /// </summary>
    /// <param name="id">Unique identifier of the sale</param>
    /// <returns>Sale details including vehicle, customer, and payment information</returns>
    /// <response code="200">Returns the sale details</response>
    /// <response code="404">Sale not found with the specified ID</response>
    /// <response code="400">Invalid sale ID format</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SaleDto>> GetById([Required] Guid id)
    {
        var sale = await _saleService.GetByIdAsync(id);
        if (sale == null)
        {
            return NotFound();
        }
        return Ok(sale);
    }

    /// <summary>
    /// Create a new sale (direct sales creation)
    /// </summary>
    /// <param name="createSaleDto">Complete sale data including customer and vehicle IDs</param>
    /// <returns>Created sale with all related information</returns>
    /// <response code="201">Sale created successfully</response>
    /// <response code="400">Invalid sale data or validation errors</response>
    /// <response code="404">Referenced vehicle, customer, or salesperson not found</response>
    /// <response code="500">Internal server error occurred</response>
    /// <remarks>
    /// This endpoint is for direct sales creation when you already have customer and salesperson IDs.
    /// For marketplace sales with CPF validation, use the /register endpoint instead.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SaleDto>> Create([FromBody, Required] CreateSaleDto createSaleDto)
    {
        var sale = await _saleService.CreateAsync(createSaleDto);
        return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
    }

    /// <summary>
    /// Update an existing sale
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <param name="updateSaleDto">Updated sale data</param>
    /// <returns>Updated sale</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<SaleDto>> Update(Guid id, UpdateSaleDto updateSaleDto)
    {
        var sale = await _saleService.UpdateAsync(id, updateSaleDto);
        return Ok(sale);
    }

    /// <summary>
    /// Delete a sale
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _saleService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Register a new sale with CPF validation and automatic vehicle status update
    /// </summary>
    /// <param name="registerSaleDto">Sale registration data including customer CPF, vehicle ID, and payment method</param>
    /// <returns>Created sale with customer and payment information including transaction ID</returns>
    /// <response code="201">Sale registered successfully. Vehicle status updated to Reserved, payment created as Pending with transaction ID.</response>
    /// <response code="400">Invalid CPF format, vehicle not available, or validation errors</response>
    /// <response code="404">Vehicle not found with the specified ID</response>
    /// <response code="409">Vehicle is not available for sale (already sold or reserved)</response>
    /// <response code="500">Internal server error occurred</response>
    /// <remarks>
    /// Marketplace Sales Process:
    /// 1. Validates CPF format (11 digits, valid algorithm)
    /// 2. Finds existing customer by CPF or creates new customer record
    /// 3. Verifies vehicle is Available (status = 1)
    /// 4. Updates vehicle status to Reserved (status = 2)
    /// 5. Creates sale record linking vehicle and customer
    /// 6. Creates payment record with Pending status and unique transaction ID
    /// 7. Returns payment URL for external processing (if applicable)
    /// 
    /// Required fields:
    /// - CustomerCpf: Valid Brazilian CPF (11 digits)
    /// - VehicleId: Must reference an existing, available vehicle
    /// - PaymentType: Payment method (1=Cash, 2=CreditCard, 3=DebitCard, 4=BankTransfer, 5=Financing, 6=Check)
    /// - SaleDate: Transaction date (defaults to current UTC time)
    /// 
    /// Optional fields:
    /// - CustomerName: Used if creating new customer (2-100 characters)
    /// - CustomerEmail: Valid email format (up to 100 characters)
    /// - CustomerPhone: Valid phone format (up to 20 characters)
    /// 
    /// Response includes:
    /// - Sale: Complete sale information
    /// - Payment: Payment record with transaction ID
    /// - TransactionId: Unique ID for webhook confirmation (format: PAY_{saleId})
    /// - PaymentUrl: External payment processing URL (if applicable for payment method)
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterSaleResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegisterSaleResponseDto>> RegisterSale([FromBody, Required] RegisterSaleDto registerSaleDto)
    {
        var response = await _saleService.RegisterSaleAsync(registerSaleDto);
        return CreatedAtAction(nameof(GetById), new { id = response.Sale.Id }, response);
    }
}