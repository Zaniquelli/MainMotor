using MainMotor.Application.DTOs;

namespace MainMotor.Application.Interfaces;

public interface ISalespersonService
{
    Task<IEnumerable<SalespersonDto>> GetAllAsync();
    Task<SalespersonDto?> GetByIdAsync(Guid id);
    Task<SalespersonDto> CreateAsync(CreateSalespersonDto createSalespersonDto);
    Task<SalespersonDto?> UpdateAsync(Guid id, UpdateSalespersonDto updateSalespersonDto);
    Task<bool> DeleteAsync(Guid id);
}