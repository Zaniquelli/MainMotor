using MainMotor.Application.DTOs;

namespace MainMotor.Application.Interfaces;

public interface IPaymentService
{
    Task<IEnumerable<PaymentDto>> GetAllAsync();
    Task<PaymentDto?> GetByIdAsync(Guid id);
    Task<PaymentDto> CreateAsync(CreatePaymentDto createPaymentDto);
    Task<PaymentDto> UpdateAsync(Guid id, UpdatePaymentDto updatePaymentDto);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<PaymentDto>> GetBySaleIdAsync(Guid saleId);
    Task ProcessWebhookAsync(PaymentWebhookDto webhookDto);
}