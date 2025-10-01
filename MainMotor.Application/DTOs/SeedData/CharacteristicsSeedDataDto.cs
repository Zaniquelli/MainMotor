namespace MainMotor.Application.DTOs.SeedData;

public class CharacteristicsSeedDataDto
{
    public List<CharacteristicTypeSeedDataDto> CharacteristicTypes { get; set; } = new();
}

public class CharacteristicTypeSeedDataDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<CharacteristicSeedDataDto> Characteristics { get; set; } = new();
}

public class CharacteristicSeedDataDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}