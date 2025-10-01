using MainMotor.Domain.Entities;

namespace MainMotor.Domain.Repositories;

public interface IModelRepository : IBaseRepository<Model>
{
    Task<IEnumerable<Model>> GetActiveAsync();
    Task<IEnumerable<Model>> GetByBrandAsync(Guid brandId);
    Task<IEnumerable<Model>> GetActiveByBrandAsync(Guid brandId);
}