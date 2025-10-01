using MainMotor.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MainMotor.Application.DTOs;

public class PaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentType PaymentType { get; set; }
    public PaymentStatus Status { get; set; }
    public string? TransactionId { get; set; }
    public string? Notes { get; set; }
    public Guid SaleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreatePaymentDto
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Payment date is required")]
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "Payment type is required")]
    public PaymentType PaymentType { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    [StringLength(100, ErrorMessage = "Transaction ID cannot exceed 100 characters")]
    public string? TransactionId { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }

    [Required(ErrorMessage = "Sale ID is required")]
    public Guid SaleId { get; set; }
}

public class UpdatePaymentDto
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Payment date is required")]
    public DateTime PaymentDate { get; set; }

    [Required(ErrorMessage = "Payment type is required")]
    public PaymentType PaymentType { get; set; }

    [Required(ErrorMessage = "Payment status is required")]
    public PaymentStatus Status { get; set; }

    [StringLength(100, ErrorMessage = "Transaction ID cannot exceed 100 characters")]
    public string? TransactionId { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }

    [Required(ErrorMessage = "Sale ID is required")]
    public Guid SaleId { get; set; }
}