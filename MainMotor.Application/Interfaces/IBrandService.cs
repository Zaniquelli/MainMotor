using MainMotor.Application.DTOs;

namespace MainMotor.Application.Interfaces;

public interface IBrandService
{
    Task<IEnumerable<BrandDto>> GetAllAsync();
}