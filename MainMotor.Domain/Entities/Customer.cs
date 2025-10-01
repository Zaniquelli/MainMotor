namespace MainMotor.Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Document { get; set; } // CPF/CNPJ
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}