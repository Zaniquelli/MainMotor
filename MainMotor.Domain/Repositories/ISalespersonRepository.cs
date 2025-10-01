using MainMotor.Domain.Entities;

namespace MainMotor.Domain.Repositories;

public interface ISalespersonRepository : IBaseRepository<Salesperson>
{
    Task<Salesperson?> GetByEmailAsync(string email);
    Task<Salesperson?> GetByEmployeeCodeAsync(string employeeCode);
    Task<IEnumerable<Salesperson>> GetActiveSalespeopleAsync();
}