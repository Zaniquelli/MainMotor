using MainMotor.Domain.Entities;
using MainMotor.Domain.Enums;

namespace MainMotor.Domain.Repositories;

public interface IVehicleRepository : IBaseRepository<Vehicle>
{
    Task<Vehicle?> GetByVinNumberAsync(string vinNumber);
    Task<Vehicle?> GetByLicensePlateAsync(string licensePlate);
    Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();
    Task<IEnumerable<Vehicle>> GetVehiclesByStatusAsync(VehicleStatus status, bool orderByPrice = false);
    Task<Vehicle?> GetVehicleBySaleIdAsync(Guid saleId);
    Task<bool> VinExistsAsync(string vinNumber);
    Task<bool> LicensePlateExistsAsync(string licensePlate);
}