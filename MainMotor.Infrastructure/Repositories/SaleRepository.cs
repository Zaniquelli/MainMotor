using Microsoft.EntityFrameworkCore;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Repositories;
using MainMotor.Infrastructure.Data;

namespace MainMotor.Infrastructure.Repositories;

public class SaleRepository : BaseRepository<Sale>, ISaleRepository
{
    public SaleRepository(MainMotorDbContext context) : base(context)
    {
    }

    public override async Task<Sale?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(s => s.Vehicle)
                .ThenInclude(v => v.ModelYear)
                    .ThenInclude(my => my.Model)
                        .ThenInclude(m => m.Brand)
            .Include(s => s.Customer)
            .Include(s => s.Salesperson)
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public override async Task<IEnumerable<Sale>> GetAllAsync()
    {
        return await _dbSet
            .Include(s => s.Vehicle)
                .ThenInclude(v => v.ModelYear)
                    .ThenInclude(my => my.Model)
                        .ThenInclude(m => m.Brand)
            .Include(s => s.Customer)
            .Include(s => s.Salesperson)
            .Include(s => s.Payments)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetSalesByCustomerIdAsync(Guid customerId)
    {
        return await _dbSet
            .Include(s => s.Vehicle)
                .ThenInclude(v => v.ModelYear)
                    .ThenInclude(my => my.Model)
                        .ThenInclude(m => m.Brand)
            .Include(s => s.Customer)
            .Include(s => s.Salesperson)
            .Include(s => s.Payments)
            .Where(s => s.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetSalesBySalespersonIdAsync(Guid salespersonId)
    {
        return await _dbSet
            .Include(s => s.Vehicle)
                .ThenInclude(v => v.ModelYear)
                    .ThenInclude(my => my.Model)
                        .ThenInclude(m => m.Brand)
            .Include(s => s.Customer)
            .Include(s => s.Salesperson)
            .Include(s => s.Payments)
            .Where(s => s.SalespersonId == salespersonId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetSalesByVehicleIdAsync(Guid vehicleId)
    {
        return await _dbSet
            .Include(s => s.Vehicle)
                .ThenInclude(v => v.ModelYear)
                    .ThenInclude(my => my.Model)
                        .ThenInclude(m => m.Brand)
            .Include(s => s.Customer)
            .Include(s => s.Salesperson)
            .Include(s => s.Payments)
            .Where(s => s.VehicleId == vehicleId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(s => s.Vehicle)
                .ThenInclude(v => v.ModelYear)
                    .ThenInclude(my => my.Model)
                        .ThenInclude(m => m.Brand)
            .Include(s => s.Customer)
            .Include(s => s.Salesperson)
            .Include(s => s.Payments)
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
            .ToListAsync();
    }
}