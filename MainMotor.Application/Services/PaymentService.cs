using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;
using MainMotor.Application.Exceptions;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Enums;
using MainMotor.Domain.Repositories;

namespace MainMotor.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IVehicleRepository _vehicleRepository;

    public PaymentService(IPaymentRepository paymentRepository, IVehicleRepository vehicleRepository)
    {
        _paymentRepository = paymentRepository;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<IEnumerable<PaymentDto>> GetAllAsync()
    {
        var payments = await _paymentRepository.GetAllAsync();
        return payments.Select(MapToDto);
    }

    public async Task<PaymentDto?> GetByIdAsync(Guid id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        return payment != null ? MapToDto(payment) : null;
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentDto createPaymentDto)
    {
        var payment = MapToEntity(createPaymentDto);
        var createdPayment = await _paymentRepository.AddAsync(payment);
        return MapToDto(createdPayment);
    }

    public async Task<PaymentDto> UpdateAsync(Guid id, UpdatePaymentDto updatePaymentDto)
    {
        var existingPayment = await _paymentRepository.GetByIdAsync(id);
        if (existingPayment == null)
            throw new NotFoundException("Payment", id);

        MapToEntity(updatePaymentDto, existingPayment);
        existingPayment.UpdatedAt = DateTime.UtcNow;
        
        var updatedPayment = await _paymentRepository.UpdateAsync(existingPayment);
        return MapToDto(updatedPayment);
    }

    public async Task DeleteAsync(Guid id)
    {
        var exists = await _paymentRepository.ExistsAsync(id);
        if (!exists)
            throw new NotFoundException("Payment", id);

        await _paymentRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<PaymentDto>> GetBySaleIdAsync(Guid saleId)
    {
        var payments = await _paymentRepository.GetPaymentsBySaleIdAsync(saleId);
        return payments.Select(MapToDto);
    }

    public async Task ProcessWebhookAsync(PaymentWebhookDto webhookDto)
    {
        // Find the payment by sale ID and transaction ID
        var payments = await _paymentRepository.GetPaymentsBySaleIdAsync(webhookDto.SaleId);
        var payment = payments.FirstOrDefault(p => p.TransactionId == webhookDto.TransactionId);
        
        if (payment == null)
            throw new NotFoundException("Payment", $"Transaction ID: {webhookDto.TransactionId}");

        // Find the vehicle associated with the sale
        var vehicle = await _vehicleRepository.GetVehicleBySaleIdAsync(webhookDto.SaleId);
        if (vehicle == null)
            throw new NotFoundException("Vehicle", $"Sale ID: {webhookDto.SaleId}");

        // Process webhook based on status
        switch (webhookDto.Status.ToLower())
        {
            case "paid":
                payment.Status = PaymentStatus.Completed;
                vehicle.Status = VehicleStatus.Sold;
                break;
            case "cancelled":
                payment.Status = PaymentStatus.Cancelled;
                vehicle.Status = VehicleStatus.Available;
                break;
            default:
                throw new ValidationException("Status", $"Invalid payment status: {webhookDto.Status}");
        }

        // Update entities
        payment.UpdatedAt = DateTime.UtcNow;
        vehicle.UpdatedAt = DateTime.UtcNow;
        
        await _paymentRepository.UpdateAsync(payment);
        await _vehicleRepository.UpdateAsync(vehicle);
    }

    private static PaymentDto MapToDto(Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            PaymentType = payment.PaymentType,
            Status = payment.Status,
            TransactionId = payment.TransactionId,
            Notes = payment.Notes,
            SaleId = payment.SaleId,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt
        };
    }

    private static Payment MapToEntity(CreatePaymentDto dto)
    {
        return new Payment
        {
            Amount = dto.Amount,
            PaymentDate = dto.PaymentDate,
            PaymentType = dto.PaymentType,
            Status = dto.Status,
            TransactionId = dto.TransactionId,
            Notes = dto.Notes,
            SaleId = dto.SaleId
        };
    }

    private static void MapToEntity(UpdatePaymentDto dto, Payment entity)
    {
        entity.Amount = dto.Amount;
        entity.PaymentDate = dto.PaymentDate;
        entity.PaymentType = dto.PaymentType;
        entity.Status = dto.Status;
        entity.TransactionId = dto.TransactionId;
        entity.Notes = dto.Notes;
        entity.SaleId = dto.SaleId;
    }
}