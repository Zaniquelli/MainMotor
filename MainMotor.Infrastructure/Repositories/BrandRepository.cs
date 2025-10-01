using Microsoft.EntityFrameworkCore;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Repositories;
using MainMotor.Infrastructure.Data;

namespace MainMotor.Infrastructure.Repositories;

public class BrandRepository : BaseRepository<Brand>, IBrandRepository
{
    public BrandRepository(MainMotorDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Brand>> GetActiveAsync()
    {
        return await _dbSet
            .Where(b => b.IsActive)
            .ToListAsync();
    }
}