using MainMotor.Application.DTOs;

namespace MainMotor.Application.Interfaces;

public interface ICharacteristicService
{
    Task<IEnumerable<CharacteristicDto>> GetAllAsync();
    Task<IEnumerable<CharacteristicDto>> GetByTypeAsync(Guid typeId);
}