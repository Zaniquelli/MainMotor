namespace MainMotor.Application.DTOs;

public class CharacteristicDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public Guid CharacteristicTypeId { get; set; }
    public string CharacteristicTypeName { get; set; } = string.Empty;
}