using Microsoft.AspNetCore.Mvc;
using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;
using MainMotor.API.Models;

namespace MainMotor.API.Controllers;

/// <summary>
/// Controller for managing payments and payment webhook integrations
/// </summary>
/// <remarks>
/// Handles payment processing, status tracking, and integration with external payment systems.
/// The webhook endpoint processes payment notifications to automatically update vehicle and sale status.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Get all payments
    /// </summary>
    /// <returns>List of payments</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAll()
    {
        var payments = await _paymentService.GetAllAsync();
        return Ok(payments);
    }

    /// <summary>
    /// Get payment by ID
    /// </summary>
    /// <param name="id">Payment ID</param>
    /// <returns>Payment details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentDto>> GetById(Guid id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment == null)
        {
            return NotFound();
        }
        return Ok(payment);
    }

    /// <summary>
    /// Get payments by sale ID
    /// </summary>
    /// <param name="saleId">Sale ID</param>
    /// <returns>List of payments for the sale</returns>
    [HttpGet("sale/{saleId}")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetBySaleId(Guid saleId)
    {
        var payments = await _paymentService.GetBySaleIdAsync(saleId);
        return Ok(payments);
    }

    /// <summary>
    /// Create a new payment
    /// </summary>
    /// <param name="createPaymentDto">Payment data</param>
    /// <returns>Created payment</returns>
    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Create(CreatePaymentDto createPaymentDto)
    {
        var payment = await _paymentService.CreateAsync(createPaymentDto);
        return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
    }

    /// <summary>
    /// Update an existing payment
    /// </summary>
    /// <param name="id">Payment ID</param>
    /// <param name="updatePaymentDto">Updated payment data</param>
    /// <returns>Updated payment</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<PaymentDto>> Update(Guid id, UpdatePaymentDto updatePaymentDto)
    {
        var payment = await _paymentService.UpdateAsync(id, updatePaymentDto);
        if (payment == null)
        {
            return NotFound();
        }
        return Ok(payment);
    }

    /// <summary>
    /// Delete a payment
    /// </summary>
    /// <param name="id">Payment ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _paymentService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Process payment webhook notification from external payment systems
    /// </summary>
    /// <param name="webhookDto">Webhook payload containing transaction ID, status, and sale ID</param>
    /// <returns>Confirmation message if processed successfully</returns>
    /// <response code="200">Webhook processed successfully. Payment and vehicle status updated.</response>
    /// <response code="400">Invalid webhook data, unknown status, or sale not found</response>
    /// <response code="404">Sale or payment not found with the specified IDs</response>
    /// <response code="500">Internal server error occurred during processing</response>
    /// <remarks>
    /// Payment Webhook Processing:
    /// 
    /// Status "paid" (Payment Completed):
    /// - Updates payment status to Completed (2)
    /// - Updates vehicle status to Sold (3)
    /// - Records processing timestamp
    /// 
    /// Status "cancelled" (Payment Cancelled):
    /// - Updates payment status to Cancelled (4)
    /// - Reverts vehicle status to Available (1)
    /// - Records processing timestamp
    /// 
    /// Required fields:
    /// - TransactionId: External payment system transaction identifier (1-100 characters)
    /// - Status: Must be either "paid" or "cancelled"
    /// - SaleId: Must reference an existing sale with associated payment
    /// 
    /// Security Note:
    /// In production, implement webhook signature verification to ensure requests
    /// are from authorized payment providers.
    /// </remarks>
    [HttpPost("webhook")]
    [ProducesResponseType(typeof(ApiSuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ProcessWebhook([FromBody] PaymentWebhookDto webhookDto)
    {
        if (webhookDto == null)
        {
            return BadRequest("Webhook data is required");
        }

        await _paymentService.ProcessWebhookAsync(webhookDto);
        return Ok(new ApiSuccessResponse
        {
            
                });
    }
}