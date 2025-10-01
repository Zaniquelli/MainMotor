using MainMotor.Application.DTOs;
using MainMotor.Domain.Enums;

namespace MainMotor.Application.Interfaces;

public interface IVehicleService
{
    Task<IEnumerable<VehicleDto>> GetAllAsync();
    Task<VehicleDto?> GetByIdAsync(Guid id);
    Task<VehicleDto?> GetVehicleByIdAsync(Guid id);
    Task<VehicleDto> CreateAsync(CreateVehicleDto createVehicleDto);
    Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto createVehicleDto);
    Task<VehicleDto> UpdateAsync(Guid id, UpdateVehicleDto updateVehicleDto);
    Task<VehicleDto> UpdateVehicleAsync(Guid id, UpdateVehicleDto updateVehicleDto);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<VehicleDto>> GetVehiclesByStatusAsync(VehicleStatus? status = null, bool orderByPrice = false);
}