using MainMotor.Domain.Entities;

namespace MainMotor.Domain.Repositories;

public interface IBrandRepository : IBaseRepository<Brand>
{
    Task<IEnumerable<Brand>> GetActiveAsync();
}