using MainMotor.Application.DTOs;

namespace MainMotor.Application.Interfaces;

public interface ICharacteristicTypeService
{
    Task<IEnumerable<CharacteristicTypeDto>> GetAllAsync();
}