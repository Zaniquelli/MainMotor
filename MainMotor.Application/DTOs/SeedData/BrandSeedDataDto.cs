namespace MainMotor.Application.DTOs.SeedData;

public class BrandSeedDataDto
{
    public string Name { get; set; } = string.Empty;
    public string FipeCode { get; set; } = string.Empty;
    public List<ModelSeedDataDto> Models { get; set; } = new();
}

public class ModelSeedDataDto
{
    public string Name { get; set; } = string.Empty;
    public string FipeCode { get; set; } = string.Empty;
    public List<ModelYearSeedDataDto> Years { get; set; } = new();
}

public class ModelYearSeedDataDto
{
    public int Year { get; set; }
    public string FipeCode { get; set; } = string.Empty;
    public string? FuelType { get; set; }
}