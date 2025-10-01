namespace MainMotor.Domain.Entities;

public class CharacteristicType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<Characteristic> Characteristics { get; set; } = new List<Characteristic>();
}