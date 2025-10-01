using MainMotor.Domain.Entities;

namespace MainMotor.Domain.Repositories;

public interface ISaleRepository : IBaseRepository<Sale>
{
    Task<IEnumerable<Sale>> GetSalesByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<Sale>> GetSalesBySalespersonIdAsync(Guid salespersonId);
    Task<IEnumerable<Sale>> GetSalesByVehicleIdAsync(Guid vehicleId);
    Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
}