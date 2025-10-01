using Microsoft.EntityFrameworkCore;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Repositories;
using MainMotor.Infrastructure.Data;

namespace MainMotor.Infrastructure.Repositories;

public class CharacteristicTypeRepository : BaseRepository<CharacteristicType>, ICharacteristicTypeRepository
{
    public CharacteristicTypeRepository(MainMotorDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CharacteristicType>> GetActiveAsync()
    {
        return await _dbSet
            .Where(ct => ct.IsActive)
            .ToListAsync();
    }
}