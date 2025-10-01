namespace MainMotor.Domain.Entities;

public class Model : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public string FipeCode { get; set; } = string.Empty;
    
    // Foreign keys
    public Guid BrandId { get; set; }
    public Guid VehicleCategoryId { get; set; }
    
    // Navigation properties
    public virtual Brand Brand { get; set; } = null!;
    public virtual VehicleCategory VehicleCategory { get; set; } = null!;
    public virtual ICollection<ModelYear> ModelYears { get; set; } = new List<ModelYear>();
}