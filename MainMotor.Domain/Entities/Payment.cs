using MainMotor.Domain.Enums;

namespace MainMotor.Domain.Entities;

public class Payment : BaseEntity
{
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public PaymentType PaymentType { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionId { get; set; }
    public string? Notes { get; set; }

    // Foreign key
    public Guid SaleId { get; set; }

    // Navigation properties
    public virtual Sale Sale { get; set; } = null!;
}