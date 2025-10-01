using Microsoft.EntityFrameworkCore;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Repositories;
using MainMotor.Infrastructure.Data;

namespace MainMotor.Infrastructure.Repositories;

public class CharacteristicRepository : BaseRepository<Characteristic>, ICharacteristicRepository
{
    public CharacteristicRepository(MainMotorDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Characteristic>> GetByTypeAsync(Guid typeId)
    {
        return await _dbSet
            .Include(c => c.CharacteristicType)
            .Where(c => c.CharacteristicTypeId == typeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Characteristic>> GetActiveAsync()
    {
        return await _dbSet
            .Include(c => c.CharacteristicType)
            .Where(c => c.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Characteristic>> GetActiveByTypeAsync(Guid typeId)
    {
        return await _dbSet
            .Include(c => c.CharacteristicType)
            .Where(c => c.CharacteristicTypeId == typeId && c.IsActive)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Characteristic>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.CharacteristicType)
            .ToListAsync();
    }

    public override async Task<Characteristic?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(c => c.CharacteristicType)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}