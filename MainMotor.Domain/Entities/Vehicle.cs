using MainMotor.Domain.Enums;

namespace MainMotor.Domain.Entities;

public class Vehicle : BaseEntity
{
    public string VinNumber { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public int Mileage { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public VehicleStatus Status { get; set; } = VehicleStatus.Available;
    public string? Notes { get; set; }
    
    // Foreign key
    public Guid ModelYearId { get; set; }
    
    // Navigation properties
    public virtual ModelYear ModelYear { get; set; } = null!;
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public virtual ICollection<Characteristic> Characteristics { get; set; } = new List<Characteristic>();
}