using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Repositories;

namespace MainMotor.Application.Services;

public class CharacteristicService : ICharacteristicService
{
    private readonly ICharacteristicRepository _characteristicRepository;

    public CharacteristicService(ICharacteristicRepository characteristicRepository)
    {
        _characteristicRepository = characteristicRepository;
    }

    public async Task<IEnumerable<CharacteristicDto>> GetAllAsync()
    {
        var characteristics = await _characteristicRepository.GetActiveAsync();
        return characteristics.Select(MapToDto);
    }

    public async Task<IEnumerable<CharacteristicDto>> GetByTypeAsync(Guid typeId)
    {
        var characteristics = await _characteristicRepository.GetActiveByTypeAsync(typeId);
        return characteristics.Select(MapToDto);
    }

    private static CharacteristicDto MapToDto(Characteristic characteristic)
    {
        return new CharacteristicDto
        {
            Id = characteristic.Id,
            Name = characteristic.Name,
            Description = characteristic.Description,
            IsActive = characteristic.IsActive,
            CharacteristicTypeId = characteristic.CharacteristicTypeId,
            CharacteristicTypeName = characteristic.CharacteristicType?.Name ?? string.Empty
        };
    }
}