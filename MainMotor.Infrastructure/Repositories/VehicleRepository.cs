using Microsoft.EntityFrameworkCore;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Enums;
using MainMotor.Domain.Repositories;
using MainMotor.Infrastructure.Data;

namespace MainMotor.Infrastructure.Repositories;

public class VehicleRepository : BaseRepository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(MainMotorDbContext context) : base(context)
    {
    }

    public override async Task<Vehicle?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(v => v.ModelYear)
                .ThenInclude(my => my.Model)
                    .ThenInclude(m => m.Brand)
            .Include(v => v.ModelYear)
                .ThenInclude(my => my.Model)
                    .ThenInclude(m => m.VehicleCategory)
            .Include(v => v.Characteristics)
                .ThenInclude(c => c.CharacteristicType)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public override async Task<IEnumerable<Vehicle>> GetAllAsync()
    {
        return await _dbSet
            .Include(v => v.ModelYear)
                .ThenInclude(my => my.Model)
                    .ThenInclude(m => m.Brand)
            .Include(v => v.ModelYear)
                .ThenInclude(my => my.Model)
                    .ThenInclude(m => m.VehicleCategory)
            .Include(v => v.Characteristics)
                .ThenInclude(c => c.CharacteristicType)
            .ToListAsync();
    }

    public async Task<Vehicle?> GetByVinNumberAsync(string vinNumber)
    {
        return await _dbSet
            .Include(v => v.ModelYear)
                .ThenInclude(my => my.Model)
                    .ThenInclude(m => m.Brand)
            .Include(v => v.ModelYear)
                .ThenInclude(my => my.Model)
                    .ThenInclude(m => m.VehicleCategory)
            .Include(v => v.Characteristics)
                .ThenInclude(c => c.CharacteristicType)
            .FirstOrDefaultAsync(v => v.VinNumber == vinNumber);
    }

    public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate)
    {
        return await _dbSet
            .Include(v => v.ModelYear)
                .ThenInclude(my => my.Model)
                    .ThenInclude(m => m.Brand)
            .Include(v => v.ModelYear)
                .ThenInclude(my => my.Model)
                    .ThenInclude(m => m.VehicleCategory)
            .Include(v => v.Characteristics)
                .ThenInclude(c => c.CharacteristicType)
            .FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);
    }

    public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
    {
        return await _dbSet
            .Include(v => v.ModelYear)
                .ThenInclude(my => my.Model)
                    .ThenInclude(m => m.Brand)
            .Include(v => v.ModelYear)
                .ThenInclude(my => my.Model)
                    .ThenInclude(m => m.VehicleCategory)
            .Include(v => v.Characteristics)
                .ThenInclude(c => c.CharacteristicType)
            .Where(v => v.Status == VehicleStatus.Available)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vehicle>> GetVehiclesByStatusAsync(VehicleStatus status, bool orderByPrice = false)
    {
        var query = _dbSet
            .Include(v => v.ModelYear)
                .ThenInclude(my => my.Model)
                    .ThenInclude(m => m.Brand)
            .Include(v => v.ModelYear)
                .ThenInclude(my => my.Model)
                    .ThenInclude(m => m.VehicleCategory)
            .Include(v => v.Characteristics)
                .ThenInclude(c => c.CharacteristicType)
            .Include(v => v.Sales)
            .Where(v => v.Status == status);

        if (orderByPrice)
        {
            query = query.OrderBy(v => v.SalePrice);
        }

        return await query.ToListAsync();
    }

    public async Task<Vehicle?> GetVehicleBySaleIdAsync(Guid saleId)
    {
        return await _dbSet
            .Include(v => v.ModelYear)
                .ThenInclude(my => my.Model)
                    .ThenInclude(m => m.Brand)
            .Include(v => v.ModelYear)
                .ThenInclude(my => my.Model)
                    .ThenInclude(m => m.VehicleCategory)
            .Include(v => v.Characteristics)
                .ThenInclude(c => c.CharacteristicType)
            .Include(v => v.Sales)
            .FirstOrDefaultAsync(v => v.Sales.Any(s => s.Id == saleId));
    }

    public async Task<bool> VinExistsAsync(string vinNumber)
    {
        return await _dbSet.AnyAsync(v => v.VinNumber == vinNumber);
    }

    public async Task<bool> LicensePlateExistsAsync(string licensePlate)
    {
        return await _dbSet.AnyAsync(v => v.LicensePlate == licensePlate);
    }
}