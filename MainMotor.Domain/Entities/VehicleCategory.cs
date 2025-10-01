namespace MainMotor.Domain.Entities;

public class VehicleCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}