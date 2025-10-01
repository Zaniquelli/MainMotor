using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Repositories;

namespace MainMotor.Application.Services;

public class CharacteristicTypeService : ICharacteristicTypeService
{
    private readonly ICharacteristicTypeRepository _characteristicTypeRepository;

    public CharacteristicTypeService(ICharacteristicTypeRepository characteristicTypeRepository)
    {
        _characteristicTypeRepository = characteristicTypeRepository;
    }

    public async Task<IEnumerable<CharacteristicTypeDto>> GetAllAsync()
    {
        var characteristicTypes = await _characteristicTypeRepository.GetActiveAsync();
        return characteristicTypes.Select(MapToDto);
    }

    private static CharacteristicTypeDto MapToDto(CharacteristicType characteristicType)
    {
        return new CharacteristicTypeDto
        {
            Id = characteristicType.Id,
            Name = characteristicType.Name,
            Description = characteristicType.Description,
            IsActive = characteristicType.IsActive
        };
    }
}