using Microsoft.EntityFrameworkCore;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Enums;
using MainMotor.Domain.Repositories;
using MainMotor.Infrastructure.Data;

namespace MainMotor.Infrastructure.Repositories;

public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(MainMotorDbContext context) : base(context)
    {
    }

    public override async Task<Payment?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Sale)
                .ThenInclude(s => s.Vehicle)
            .Include(p => p.Sale)
                .ThenInclude(s => s.Customer)
            .Include(p => p.Sale)
                .ThenInclude(s => s.Salesperson)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<IEnumerable<Payment>> GetAllAsync()
    {
        return await _dbSet
            .Include(p => p.Sale)
                .ThenInclude(s => s.Vehicle)
            .Include(p => p.Sale)
                .ThenInclude(s => s.Customer)
            .Include(p => p.Sale)
                .ThenInclude(s => s.Salesperson)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetPaymentsBySaleIdAsync(Guid saleId)
    {
        return await _dbSet
            .Include(p => p.Sale)
            .Where(p => p.SaleId == saleId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
    {
        return await _dbSet
            .Include(p => p.Sale)
                .ThenInclude(s => s.Vehicle)
            .Include(p => p.Sale)
                .ThenInclude(s => s.Customer)
            .Include(p => p.Sale)
                .ThenInclude(s => s.Salesperson)
            .Where(p => p.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(p => p.Sale)
                .ThenInclude(s => s.Vehicle)
            .Include(p => p.Sale)
                .ThenInclude(s => s.Customer)
            .Include(p => p.Sale)
                .ThenInclude(s => s.Salesperson)
            .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalPaymentsBySaleIdAsync(Guid saleId)
    {
        return await _dbSet
            .Where(p => p.SaleId == saleId && p.Status == PaymentStatus.Completed)
            .SumAsync(p => p.Amount);
    }
}