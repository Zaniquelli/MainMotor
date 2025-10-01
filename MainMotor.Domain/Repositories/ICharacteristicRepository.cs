using MainMotor.Domain.Entities;

namespace MainMotor.Domain.Repositories;

public interface ICharacteristicRepository : IBaseRepository<Characteristic>
{
    Task<IEnumerable<Characteristic>> GetByTypeAsync(Guid typeId);
    Task<IEnumerable<Characteristic>> GetActiveAsync();
    Task<IEnumerable<Characteristic>> GetActiveByTypeAsync(Guid typeId);
}