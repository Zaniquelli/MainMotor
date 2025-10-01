using Microsoft.EntityFrameworkCore;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Repositories;
using MainMotor.Infrastructure.Data;

namespace MainMotor.Infrastructure.Repositories;

public class ModelRepository : BaseRepository<Model>, IModelRepository
{
    public ModelRepository(MainMotorDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Model>> GetActiveAsync()
    {
        return await _dbSet
            .Include(m => m.Brand)
            .Where(m => m.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Model>> GetByBrandAsync(Guid brandId)
    {
        return await _dbSet
            .Include(m => m.Brand)
            .Where(m => m.BrandId == brandId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Model>> GetActiveByBrandAsync(Guid brandId)
    {
        return await _dbSet
            .Include(m => m.Brand)
            .Where(m => m.BrandId == brandId && m.IsActive)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Model>> GetAllAsync()
    {
        return await _dbSet
            .Include(m => m.Brand)
            .ToListAsync();
    }

    public override async Task<Model?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(m => m.Brand)
            .FirstOrDefaultAsync(m => m.Id == id);
    }
}