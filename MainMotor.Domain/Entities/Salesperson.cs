namespace MainMotor.Domain.Entities;

public class Salesperson : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? EmployeeCode { get; set; }
    public decimal CommissionRate { get; set; } = 0.05m; // Default 5%
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}