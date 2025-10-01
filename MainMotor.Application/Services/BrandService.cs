using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Repositories;

namespace MainMotor.Application.Services;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _brandRepository;

    public BrandService(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<IEnumerable<BrandDto>> GetAllAsync()
    {
        var brands = await _brandRepository.GetActiveAsync();
        return brands.Select(MapToDto);
    }

    private static BrandDto MapToDto(Brand brand)
    {
        return new BrandDto
        {
            Id = brand.Id,
            Name = brand.Name,
            Description = brand.Description,
            IsActive = brand.IsActive,
            FipeCode = brand.FipeCode
        };
    }
}