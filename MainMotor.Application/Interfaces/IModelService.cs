using MainMotor.Application.DTOs;

namespace MainMotor.Application.Interfaces;

public interface IModelService
{
    Task<IEnumerable<ModelDto>> GetAllAsync();
    Task<IEnumerable<ModelDto>> GetByBrandAsync(Guid brandId);
}