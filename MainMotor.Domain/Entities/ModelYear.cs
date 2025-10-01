namespace MainMotor.Domain.Entities;

public class ModelYear : BaseEntity
{
    public int Year { get; set; }
    public bool IsActive { get; set; } = true;
    public string FipeCode { get; set; } = string.Empty;
    
    // Foreign key
    public Guid ModelId { get; set; }
    
    // Navigation properties
    public virtual Model Model { get; set; } = null!;
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public virtual ICollection<FipePriceHistory> FipePriceHistories { get; set; } = new List<FipePriceHistory>();
}