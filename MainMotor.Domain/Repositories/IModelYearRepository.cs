using MainMotor.Domain.Entities;

namespace MainMotor.Domain.Repositories;

public interface IModelYearRepository : IBaseRepository<ModelYear>
{
    Task<IEnumerable<ModelYear>> GetByModelIdAsync(Guid modelId);
    Task<IEnumerable<ModelYear>> GetByYearAsync(int year);
}