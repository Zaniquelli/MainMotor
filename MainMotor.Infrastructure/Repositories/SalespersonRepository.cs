using Microsoft.EntityFrameworkCore;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Repositories;
using MainMotor.Infrastructure.Data;

namespace MainMotor.Infrastructure.Repositories;

public class SalespersonRepository : BaseRepository<Salesperson>, ISalespersonRepository
{
    public SalespersonRepository(MainMotorDbContext context) : base(context)
    {
    }

    public async Task<Salesperson?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<Salesperson?> GetByEmployeeCodeAsync(string employeeCode)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.EmployeeCode == employeeCode);
    }

    public async Task<IEnumerable<Salesperson>> GetActiveSalespeopleAsync()
    {
        return await _dbSet
            .Where(s => s.IsActive)
            .ToListAsync();
    }
}