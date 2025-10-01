namespace MainMotor.Domain.Entities;

public class FipePriceHistory : BaseEntity
{
    public decimal Price { get; set; }
    public DateTime ReferenceDate { get; set; }
    public string? FipeCode { get; set; }
    
    // Foreign key
    public Guid ModelYearId { get; set; }
    
    // Navigation properties
    public virtual ModelYear ModelYear { get; set; } = null!;
}