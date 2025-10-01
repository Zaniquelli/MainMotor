using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;
using MainMotor.Application.Exceptions;
using MainMotor.Application.Services;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Enums;
using MainMotor.Domain.Repositories;

namespace MainMotor.Application.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ICharacteristicRepository _characteristicRepository;
    private readonly IValidationService _validationService;

    public VehicleService(IVehicleRepository vehicleRepository, ICharacteristicRepository characteristicRepository, IValidationService validationService)
    {
        _vehicleRepository = vehicleRepository;
        _characteristicRepository = characteristicRepository;
        _validationService = validationService;
    }

    public async Task<IEnumerable<VehicleDto>> GetAllAsync()
    {
        var vehicles = await _vehicleRepository.GetAllAsync();
        return vehicles.Select(MapToDto);
    }

    public async Task<VehicleDto?> GetByIdAsync(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        return vehicle != null ? MapToDto(vehicle) : null;
    }

    // Alias method to match task requirement naming
    public async Task<VehicleDto?> GetVehicleByIdAsync(Guid id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<VehicleDto> CreateAsync(CreateVehicleDto createVehicleDto)
    {
        // Validate business rules
        await _validationService.ValidateAsync(createVehicleDto);
        
        // Validate characteristic existence
        await ValidateCharacteristicsExistAsync(createVehicleDto.CharacteristicIds);
        
        var vehicle = await MapToEntityAsync(createVehicleDto);
        var createdVehicle = await _vehicleRepository.AddAsync(vehicle);
        return MapToDto(createdVehicle);
    }

    // Alias method to match task requirement naming
    public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto createVehicleDto)
    {
        return await CreateAsync(createVehicleDto);
    }

    public async Task<VehicleDto> UpdateAsync(Guid id, UpdateVehicleDto updateVehicleDto)
    {
        var existingVehicle = await _vehicleRepository.GetByIdAsync(id);
        if (existingVehicle == null)
            throw new NotFoundException("Vehicle", id);

        // Only allow editing if vehicle status is Available
        if (existingVehicle.Status != VehicleStatus.Available)
            throw new ConflictException("Vehicle can only be edited when status is Available");

        // Validate characteristic existence
        await ValidateCharacteristicsExistAsync(updateVehicleDto.CharacteristicIds);

        await MapToEntityAsync(updateVehicleDto, existingVehicle);
        existingVehicle.UpdatedAt = DateTime.UtcNow;
        
        var updatedVehicle = await _vehicleRepository.UpdateAsync(existingVehicle);
        return MapToDto(updatedVehicle);
    }

    // Alias method to match task requirement naming
    public async Task<VehicleDto> UpdateVehicleAsync(Guid id, UpdateVehicleDto updateVehicleDto)
    {
        return await UpdateAsync(id, updateVehicleDto);
    }

    public async Task DeleteAsync(Guid id)
    {
        var exists = await _vehicleRepository.ExistsAsync(id);
        if (!exists)
            throw new NotFoundException("Vehicle", id);

        await _vehicleRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<VehicleDto>> GetVehiclesByStatusAsync(VehicleStatus? status = null, bool orderByPrice = false)
    {
        IEnumerable<Vehicle> vehicles;
        
        if (status.HasValue)
        {
            vehicles = await _vehicleRepository.GetVehiclesByStatusAsync(status.Value, orderByPrice);
        }
        else
        {
            vehicles = await _vehicleRepository.GetAllAsync();
            if (orderByPrice)
            {
                vehicles = vehicles.OrderBy(v => v.SalePrice);
            }
        }
        
        return vehicles.Select(MapToDto);
    }

    private static VehicleDto MapToDto(Vehicle vehicle)
    {
        return new VehicleDto
        {
            Id = vehicle.Id,
            VinNumber = vehicle.VinNumber,
            LicensePlate = vehicle.LicensePlate,
            Mileage = vehicle.Mileage,
            PurchasePrice = vehicle.PurchasePrice,
            SalePrice = vehicle.SalePrice,
            Status = vehicle.Status,
            Notes = vehicle.Notes,
            ModelYearId = vehicle.ModelYearId,
            CreatedAt = vehicle.CreatedAt,
            UpdatedAt = vehicle.UpdatedAt,
            BrandName = vehicle.ModelYear?.Model?.Brand?.Name ?? string.Empty,
            ModelName = vehicle.ModelYear?.Model?.Name ?? string.Empty,
            Year = vehicle.ModelYear?.Year ?? 0,
            SaleDate = vehicle.Sales?.FirstOrDefault()?.SaleDate,
            Characteristics = vehicle.Characteristics?.Select(c => new VehicleCharacteristicDto
            {
                CharacteristicId = c.Id,
                CharacteristicName = c.Name,
                CharacteristicTypeName = c.CharacteristicType?.Name ?? string.Empty
            }).ToList() ?? new List<VehicleCharacteristicDto>()
        };
    }

    private async Task<Vehicle> MapToEntityAsync(CreateVehicleDto dto)
    {
        var characteristics = await GetCharacteristicsByIdsAsync(dto.CharacteristicIds);
        
        return new Vehicle
        {
            VinNumber = dto.VinNumber,
            LicensePlate = dto.LicensePlate,
            Mileage = dto.Mileage,
            PurchasePrice = dto.PurchasePrice,
            SalePrice = dto.SalePrice,
            Status = dto.Status,
            Notes = dto.Notes,
            ModelYearId = dto.ModelYearId,
            Characteristics = characteristics.ToList()
        };
    }

    private async Task MapToEntityAsync(UpdateVehicleDto dto, Vehicle entity)
    {
        entity.VinNumber = dto.VinNumber;
        entity.LicensePlate = dto.LicensePlate;
        entity.Mileage = dto.Mileage;
        entity.PurchasePrice = dto.PurchasePrice;
        entity.SalePrice = dto.SalePrice;
        entity.Status = dto.Status;
        entity.Notes = dto.Notes;
        entity.ModelYearId = dto.ModelYearId;

        // Update characteristics
        var characteristics = await GetCharacteristicsByIdsAsync(dto.CharacteristicIds);
        entity.Characteristics.Clear();
        foreach (var characteristic in characteristics)
        {
            entity.Characteristics.Add(characteristic);
        }
    }

    private async Task ValidateCharacteristicsExistAsync(ICollection<Guid> characteristicIds)
    {
        if (characteristicIds == null || !characteristicIds.Any())
        {
            throw new ValidationException("At least one characteristic must be selected");
        }

        var existingCharacteristics = await GetCharacteristicsByIdsAsync(characteristicIds);
        var existingIds = existingCharacteristics.Select(c => c.Id).ToHashSet();
        var missingIds = characteristicIds.Where(id => !existingIds.Contains(id)).ToList();

        if (missingIds.Any())
        {
            throw new NotFoundException($"Characteristics not found: {string.Join(", ", missingIds)}");
        }

        // Validate that all characteristics are active
        var inactiveCharacteristics = existingCharacteristics.Where(c => !c.IsActive).ToList();
        if (inactiveCharacteristics.Any())
        {
            var inactiveNames = string.Join(", ", inactiveCharacteristics.Select(c => c.Name));
            throw new ValidationException($"The following characteristics are inactive and cannot be used: {inactiveNames}");
        }
    }

    private async Task<IEnumerable<Characteristic>> GetCharacteristicsByIdsAsync(ICollection<Guid> characteristicIds)
    {
        var characteristics = new List<Characteristic>();
        foreach (var id in characteristicIds)
        {
            var characteristic = await _characteristicRepository.GetByIdAsync(id);
            if (characteristic != null)
            {
                characteristics.Add(characteristic);
            }
        }
        return characteristics;
    }  }