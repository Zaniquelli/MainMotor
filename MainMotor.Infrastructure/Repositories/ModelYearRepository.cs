using Microsoft.EntityFrameworkCore;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Repositories;
using MainMotor.Infrastructure.Data;

namespace MainMotor.Infrastructure.Repositories;

public class ModelYearRepository : BaseRepository<ModelYear>, IModelYearRepository
{
    public ModelYearRepository(MainMotorDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ModelYear>> GetByModelIdAsync(Guid modelId)
    {
        return await _dbSet
            .Include(my => my.Model)
                .ThenInclude(m => m.Brand)
            .Where(my => my.ModelId == modelId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ModelYear>> GetByYearAsync(int year)
    {
        return await _dbSet
            .Include(my => my.Model)
                .ThenInclude(m => m.Brand)
            .Where(my => my.Year == year)
            .ToListAsync();
    }
}