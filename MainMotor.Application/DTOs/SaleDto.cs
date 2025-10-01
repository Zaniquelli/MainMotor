using System.ComponentModel.DataAnnotations;
using MainMotor.Domain.Enums;

namespace MainMotor.Application.DTOs;

public class SaleDto
{
    public Guid Id { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CommissionAmount { get; set; }
    public string? Notes { get; set; }
    public Guid VehicleId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid SalespersonId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateSaleDto
{
    [Required(ErrorMessage = "Sale date is required")]
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
    public decimal TotalAmount { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Commission amount cannot be negative")]
    public decimal CommissionAmount { get; set; }

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }

    [Required(ErrorMessage = "Vehicle ID is required")]
    public Guid VehicleId { get; set; }

    [Required(ErrorMessage = "Customer ID is required")]
    public Guid CustomerId { get; set; }

    [Required(ErrorMessage = "Salesperson ID is required")]
    public Guid SalespersonId { get; set; }
}

public class UpdateSaleDto
{
    [Required(ErrorMessage = "Sale date is required")]
    public DateTime SaleDate { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
    public decimal TotalAmount { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Commission amount cannot be negative")]
    public decimal CommissionAmount { get; set; }

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }

    [Required(ErrorMessage = "Vehicle ID is required")]
    public Guid VehicleId { get; set; }

    [Required(ErrorMessage = "Customer ID is required")]
    public Guid CustomerId { get; set; }

    [Required(ErrorMessage = "Salesperson ID is required")]
    public Guid SalespersonId { get; set; }
}

/// <summary>
/// Data transfer object for registering a marketplace sale with CPF validation
/// </summary>
/// <remarks>
/// Used for the marketplace sales process where customers purchase vehicles using their CPF.
/// Automatically handles customer creation/lookup and vehicle status updates.
/// </remarks>
public class RegisterSaleDto
{
    /// <summary>
    /// Customer's CPF (Brazilian tax ID) - must be valid 11-digit CPF
    /// </summary>
    /// <example>12345678901</example>
    [Required(ErrorMessage = "Customer CPF is required")]
    [MainMotor.Application.Validators.CpfValidation(ErrorMessage = "Invalid CPF format")]
    public string CustomerCpf { get; set; } = string.Empty;

    /// <summary>
    /// Date and time of the sale transaction
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    [Required(ErrorMessage = "Sale date is required")]
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ID of the vehicle being purchased - must be Available status
    /// </summary>
    /// <example>123e4567-e89b-12d3-a456-426614174000</example>
    [Required(ErrorMessage = "Vehicle ID is required")]
    public Guid VehicleId { get; set; }

    /// <summary>
    /// Customer name - used when creating new customer record (optional if customer exists)
    /// </summary>
    /// <example>João Silva</example>
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Customer name must be between 2 and 100 characters")]
    public string? CustomerName { get; set; }

    /// <summary>
    /// Customer email address - used when creating new customer record
    /// </summary>
    /// <example>joao.silva@email.com</example>
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string? CustomerEmail { get; set; }

    /// <summary>
    /// Customer phone number - used when creating new customer record
    /// </summary>
    /// <example>(11) 99999-9999</example>
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? CustomerPhone { get; set; }

    /// <summary>
    /// Payment method for the transaction
    /// </summary>
    /// <example>2</example>
    [Required(ErrorMessage = "Payment type is required")]
    public PaymentType PaymentType { get; set; } = PaymentType.CreditCard;
}

/// <summary>
/// Response DTO for marketplace sale registration including payment information
/// </summary>
public class RegisterSaleResponseDto
{
    /// <summary>
    /// Sale information
    /// </summary>
    public SaleDto Sale { get; set; } = null!;

    /// <summary>
    /// Payment information including transaction ID for external processing
    /// </summary>
    public PaymentDto Payment { get; set; } = null!;

    /// <summary>
    /// External payment transaction ID for webhook confirmation
    /// </summary>
    /// <example>PAY_123e4567-e89b-12d3-a456-426614174000</example>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Payment processing URL (if applicable for the payment method)
    /// </summary>
    /// <example>https://payment-gateway.com/process/PAY_123e4567-e89b-12d3-a456-426614174000</example>
    public string? PaymentUrl { get; set; }
}