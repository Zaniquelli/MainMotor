using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Repositories;

namespace MainMotor.Application.Services;

public class SalespersonService : ISalespersonService
{
    private readonly ISalespersonRepository _salespersonRepository;

    public SalespersonService(ISalespersonRepository salespersonRepository)
    {
        _salespersonRepository = salespersonRepository;
    }

    public async Task<IEnumerable<SalespersonDto>> GetAllAsync()
    {
        var salespeople = await _salespersonRepository.GetAllAsync();
        return salespeople.Select(MapToDto);
    }

    public async Task<SalespersonDto?> GetByIdAsync(Guid id)
    {
        var salesperson = await _salespersonRepository.GetByIdAsync(id);
        return salesperson != null ? MapToDto(salesperson) : null;
    }

    public async Task<SalespersonDto> CreateAsync(CreateSalespersonDto createSalespersonDto)
    {
        var salesperson = MapToEntity(createSalespersonDto);
        var createdSalesperson = await _salespersonRepository.AddAsync(salesperson);
        return MapToDto(createdSalesperson);
    }

    public async Task<SalespersonDto?> UpdateAsync(Guid id, UpdateSalespersonDto updateSalespersonDto)
    {
        var existingSalesperson = await _salespersonRepository.GetByIdAsync(id);
        if (existingSalesperson == null)
            return null;

        MapToEntity(updateSalespersonDto, existingSalesperson);
        existingSalesperson.UpdatedAt = DateTime.UtcNow;
        
        var updatedSalesperson = await _salespersonRepository.UpdateAsync(existingSalesperson);
        return MapToDto(updatedSalesperson);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var exists = await _salespersonRepository.ExistsAsync(id);
        if (!exists)
            return false;

        await _salespersonRepository.DeleteAsync(id);
        return true;
    }

    private static SalespersonDto MapToDto(Salesperson salesperson)
    {
        return new SalespersonDto
        {
            Id = salesperson.Id,
            Name = salesperson.Name,
            Email = salesperson.Email,
            Phone = salesperson.Phone,
            EmployeeCode = salesperson.EmployeeCode,
            CommissionRate = salesperson.CommissionRate,
            IsActive = salesperson.IsActive,
            CreatedAt = salesperson.CreatedAt,
            UpdatedAt = salesperson.UpdatedAt
        };
    }

    private static Salesperson MapToEntity(CreateSalespersonDto dto)
    {
        return new Salesperson
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            EmployeeCode = dto.EmployeeCode,
            CommissionRate = dto.CommissionRate,
            IsActive = dto.IsActive
        };
    }

    private static void MapToEntity(UpdateSalespersonDto dto, Salesperson entity)
    {
        entity.Name = dto.Name;
        entity.Email = dto.Email;
        entity.Phone = dto.Phone;
        entity.EmployeeCode = dto.EmployeeCode;
        entity.CommissionRate = dto.CommissionRate;
        entity.IsActive = dto.IsActive;
    }
}