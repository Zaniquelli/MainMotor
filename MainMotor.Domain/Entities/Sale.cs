namespace MainMotor.Domain.Entities;

public class Sale : BaseEntity
{
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public decimal CommissionAmount { get; set; }
    public string? Notes { get; set; }
    
    // Foreign keys
    public Guid VehicleId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid SalespersonId { get; set; }
    
    // Navigation properties
    public virtual Vehicle Vehicle { get; set; } = null!;
    public virtual Customer Customer { get; set; } = null!;
    public virtual Salesperson Salesperson { get; set; } = null!;
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}