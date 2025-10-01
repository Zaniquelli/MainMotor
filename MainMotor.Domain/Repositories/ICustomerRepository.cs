using MainMotor.Domain.Entities;

namespace MainMotor.Domain.Repositories;

public interface ICustomerRepository : IBaseRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetByDocumentAsync(string document);
    Task<IEnumerable<Customer>> GetActiveCustomersAsync();
}