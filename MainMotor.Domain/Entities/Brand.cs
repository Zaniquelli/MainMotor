namespace MainMotor.Domain.Entities;

public class Brand : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public string FipeCode { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}