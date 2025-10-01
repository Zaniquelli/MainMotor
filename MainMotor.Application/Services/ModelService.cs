using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Repositories;

namespace MainMotor.Application.Services;

public class ModelService : IModelService
{
    private readonly IModelRepository _modelRepository;

    public ModelService(IModelRepository modelRepository)
    {
        _modelRepository = modelRepository;
    }

    public async Task<IEnumerable<ModelDto>> GetAllAsync()
    {
        var models = await _modelRepository.GetActiveAsync();
        return models.Select(MapToDto);
    }

    public async Task<IEnumerable<ModelDto>> GetByBrandAsync(Guid brandId)
    {
        var models = await _modelRepository.GetActiveByBrandAsync(brandId);
        return models.Select(MapToDto);
    }

    private static ModelDto MapToDto(Model model)
    {
        return new ModelDto
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            IsActive = model.IsActive,
            FipeCode = model.FipeCode,
            BrandId = model.BrandId,
            BrandName = model.Brand?.Name ?? string.Empty,
            VehicleCategoryId = model.VehicleCategoryId
        };
    }
}