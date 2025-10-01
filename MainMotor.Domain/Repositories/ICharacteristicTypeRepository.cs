using MainMotor.Domain.Entities;

namespace MainMotor.Domain.Repositories;

public interface ICharacteristicTypeRepository : IBaseRepository<CharacteristicType>
{
    Task<IEnumerable<CharacteristicType>> GetActiveAsync();
}