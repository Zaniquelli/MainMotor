namespace MainMotor.Domain.Entities;

public class Characteristic : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Foreign key
    public Guid CharacteristicTypeId { get; set; }
    
    // Navigation properties
    public virtual CharacteristicType CharacteristicType { get; set; } = null!;
}