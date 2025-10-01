using MainMotor.Application.DTOs;
using MainMotor.Domain.Repositories;

namespace MainMotor.Application.Validators;

/// <summary>
/// Business rule validator for vehicle operations following Single Responsibility Principle
/// </summary>
public class VehicleBusinessRuleValidator : IBusinessRuleValidator<CreateVehicleDto>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IModelYearRepository _modelYearRepository;

    public VehicleBusinessRuleValidator(
        IVehicleRepository vehicleRepository, 
        IModelYearRepository modelYearRepository)
    {
        _vehicleRepository = vehicleRepository;
        _modelYearRepository = modelYearRepository;
    }

    public async Task<ValidationResult> ValidateAsync(CreateVehicleDto vehicle)
    {
        var errors = new List<string>();

        // Validate VIN uniqueness
        if (await _vehicleRepository.VinExistsAsync(vehicle.VinNumber))
        {
            errors.Add("VIN number already exists in the system");
        }

        // Validate license plate uniqueness
        if (await _vehicleRepository.LicensePlateExistsAsync(vehicle.LicensePlate))
        {
            errors.Add("License plate already exists in the system");
        }

        // Validate ModelYear exists
        if (!await _modelYearRepository.ExistsAsync(vehicle.ModelYearId))
        {
            errors.Add("Selected model year does not exist");
        }

        // Business rule: Sale price should be higher than purchase price
        if (vehicle.SalePrice <= vehicle.PurchasePrice)
        {
            errors.Add("Sale price must be higher than purchase price");
        }

        return errors.Any() ? ValidationResult.Failure(errors.ToArray()) : ValidationResult.Success();
    }
}