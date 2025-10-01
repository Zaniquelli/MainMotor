using MainMotor.Application.DTOs.SeedData;

namespace MainMotor.Application.Interfaces;

public interface ISeedDataReaderService
{
    Task<IEnumerable<BrandSeedDataDto>> ReadBrandModelYearDataAsync();
}