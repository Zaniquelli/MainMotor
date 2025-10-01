using System.ComponentModel.DataAnnotations;

namespace MainMotor.Application.DTOs;

/// <summary>
/// Data transfer object for payment webhook notifications from external payment systems
/// </summary>
/// <remarks>
/// Receives payment status updates that automatically trigger vehicle and sale status changes.
/// Used by external payment providers to notify the system of payment completion or cancellation.
/// </remarks>
public class PaymentWebhookDto
{
    /// <summary>
    /// External payment system transaction identifier
    /// </summary>
    /// <example>TXN123456789</example>
    [Required(ErrorMessage = "Transaction ID is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Transaction ID must be between 1 and 100 characters")]
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Payment status - must be either "paid" or "cancelled"
    /// </summary>
    /// <example>paid</example>
    [Required(ErrorMessage = "Status is required")]
    [RegularExpression(@"^(paid|cancelled)$", ErrorMessage = "Status must be either 'paid' or 'cancelled'")]
    public string Status { get; set; } = string.Empty; // "paid" or "cancelled"

    /// <summary>
    /// ID of the sale associated with this payment
    /// </summary>
    /// <example>123e4567-e89b-12d3-a456-426614174000</example>
    [Required(ErrorMessage = "Sale ID is required")]
    public Guid SaleId { get; set; }
}