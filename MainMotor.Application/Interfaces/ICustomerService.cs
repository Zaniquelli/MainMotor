using MainMotor.Application.DTOs;

namespace MainMotor.Application.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> GetByIdAsync(Guid id);
    Task<CustomerDto> CreateAsync(CreateCustomerDto createCustomerDto);
    Task<CustomerDto?> UpdateAsync(Guid id, UpdateCustomerDto updateCustomerDto);
    Task<bool> DeleteAsync(Guid id);
}