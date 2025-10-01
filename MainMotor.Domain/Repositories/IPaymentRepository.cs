using MainMotor.Domain.Entities;
using MainMotor.Domain.Enums;

namespace MainMotor.Domain.Repositories;

public interface IPaymentRepository : IBaseRepository<Payment>
{
    Task<IEnumerable<Payment>> GetPaymentsBySaleIdAsync(Guid saleId);
    Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status);
    Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalPaymentsBySaleIdAsync(Guid saleId);
}