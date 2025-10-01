using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Repositories;

namespace MainMotor.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return customers.Select(MapToDto);
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        return customer != null ? MapToDto(customer) : null;
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto createCustomerDto)
    {
        var customer = MapToEntity(createCustomerDto);
        var createdCustomer = await _customerRepository.AddAsync(customer);
        return MapToDto(createdCustomer);
    }

    public async Task<CustomerDto?> UpdateAsync(Guid id, UpdateCustomerDto updateCustomerDto)
    {
        var existingCustomer = await _customerRepository.GetByIdAsync(id);
        if (existingCustomer == null)
            return null;

        MapToEntity(updateCustomerDto, existingCustomer);
        existingCustomer.UpdatedAt = DateTime.UtcNow;
        
        var updatedCustomer = await _customerRepository.UpdateAsync(existingCustomer);
        return MapToDto(updatedCustomer);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var exists = await _customerRepository.ExistsAsync(id);
        if (!exists)
            return false;

        await _customerRepository.DeleteAsync(id);
        return true;
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            Document = customer.Document,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }

    private static Customer MapToEntity(CreateCustomerDto dto)
    {
        return new Customer
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            Document = dto.Document,
            IsActive = dto.IsActive
        };
    }

    private static void MapToEntity(UpdateCustomerDto dto, Customer entity)
    {
        entity.Name = dto.Name;
        entity.Email = dto.Email;
        entity.Phone = dto.Phone;
        entity.Address = dto.Address;
        entity.Document = dto.Document;
        entity.IsActive = dto.IsActive;
    }
}