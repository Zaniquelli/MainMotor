using MainMotor.Application.DTOs;

namespace MainMotor.Application.Interfaces;

public interface ISaleService
{
    Task<IEnumerable<SaleDto>> GetAllAsync();
    Task<SaleDto?> GetByIdAsync(Guid id);
    Task<SaleDto> CreateAsync(CreateSaleDto createSaleDto);
    Task<SaleDto> UpdateAsync(Guid id, UpdateSaleDto updateSaleDto);
    Task DeleteAsync(Guid id);
    Task<RegisterSaleResponseDto> RegisterSaleAsync(RegisterSaleDto registerSaleDto);
}