using DocumentValidator;
using MainMotor.Application.DTOs;
using MainMotor.Application.Exceptions;
using MainMotor.Application.Interfaces;
using MainMotor.Domain.Entities;
using MainMotor.Domain.Enums;
using MainMotor.Domain.Repositories;

namespace MainMotor.Application.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ISalespersonRepository _salespersonRepository;

    public SaleService(
        ISaleRepository saleRepository,
        IVehicleRepository vehicleRepository,
        ICustomerRepository customerRepository,
        IPaymentRepository paymentRepository,
        ISalespersonRepository salespersonRepository)
    {
        _saleRepository = saleRepository;
        _vehicleRepository = vehicleRepository;
        _customerRepository = customerRepository;
        _paymentRepository = paymentRepository;
        _salespersonRepository = salespersonRepository;
    }

    public async Task<IEnumerable<SaleDto>> GetAllAsync()
    {
        var sales = await _saleRepository.GetAllAsync();
        return sales.Select(MapToDto);
    }

    public async Task<SaleDto?> GetByIdAsync(Guid id)
    {
        var sale = await _saleRepository.GetByIdAsync(id);
        return sale != null ? MapToDto(sale) : null;
    }

    public async Task<SaleDto> CreateAsync(CreateSaleDto createSaleDto)
    {
        var sale = MapToEntity(createSaleDto);
        var createdSale = await _saleRepository.AddAsync(sale);
        return MapToDto(createdSale);
    }

    public async Task<SaleDto> UpdateAsync(Guid id, UpdateSaleDto updateSaleDto)
    {
        var existingSale = await _saleRepository.GetByIdAsync(id);
        if (existingSale == null)
            throw new NotFoundException("Sale", id);

        MapToEntity(updateSaleDto, existingSale);
        existingSale.UpdatedAt = DateTime.UtcNow;
        
        var updatedSale = await _saleRepository.UpdateAsync(existingSale);
        return MapToDto(updatedSale);
    }

    public async Task DeleteAsync(Guid id)
    {
        var exists = await _saleRepository.ExistsAsync(id);
        if (!exists)
            throw new NotFoundException("Sale", id);

        await _saleRepository.DeleteAsync(id);
    }

    public async Task<RegisterSaleResponseDto> RegisterSaleAsync(RegisterSaleDto registerSaleDto)
    {
        // Validate CPF format
        if (!CpfValidation.Validate(registerSaleDto.CustomerCpf))
        {
            throw new ValidationException("CustomerCpf", "Invalid CPF format");
        }

        var cleanCpf = new string([.. registerSaleDto.CustomerCpf.Where(char.IsDigit)]);

        // Check vehicle availability
        var vehicle = await _vehicleRepository.GetByIdAsync(registerSaleDto.VehicleId);
        if (vehicle == null)
        {
            throw new NotFoundException("Vehicle", registerSaleDto.VehicleId);
        }

        if (vehicle.Status != VehicleStatus.Available)
        {
            throw new ConflictException("Vehicle is not available for sale");
        }

        // Find or create customer
        var customer = await _customerRepository.GetByDocumentAsync(cleanCpf);
        if (customer == null)
        {
            customer = new Customer
            {
                Document = cleanCpf,
                Name = registerSaleDto.CustomerName ?? "Customer",
                Email = registerSaleDto.CustomerEmail ?? string.Empty,
                Phone = registerSaleDto.CustomerPhone,
                IsActive = true
            };
            customer = await _customerRepository.AddAsync(customer);
        }

        // Update vehicle status to Reserved
        vehicle.Status = VehicleStatus.Reserved;
        await _vehicleRepository.UpdateAsync(vehicle);

        // Create sale record (we need a default salesperson - let's get the first one or create a default)
        var defaultSalespersonId = await GetDefaultSalespersonIdAsync();
        
        var sale = new Sale
        {
            VehicleId = registerSaleDto.VehicleId,
            CustomerId = customer.Id,
            SalespersonId = defaultSalespersonId,
            SaleDate = registerSaleDto.SaleDate,
            TotalAmount = vehicle.SalePrice,
            CommissionAmount = 0, // Default commission
            Notes = "Sale registered via marketplace"
        };

        var createdSale = await _saleRepository.AddAsync(sale);

        // Generate unique transaction ID for external payment processing
        var transactionId = $"PAY_{createdSale.Id:N}";

        // Create payment record with Pending status and specified payment type
        var payment = new Payment
        {
            SaleId = createdSale.Id,
            Amount = vehicle.SalePrice,
            PaymentDate = registerSaleDto.SaleDate,
            PaymentType = registerSaleDto.PaymentType,
            Status = PaymentStatus.Pending,
            TransactionId = transactionId,
            Notes = $"Payment pending for marketplace sale - {registerSaleDto.PaymentType}"
        };

        var createdPayment = await _paymentRepository.AddAsync(payment);

        // Generate payment URL based on payment type (mock implementation)
        var paymentUrl = GeneratePaymentUrl(registerSaleDto.PaymentType, transactionId);

        return new RegisterSaleResponseDto
        {
            Sale = MapToDto(createdSale),
            Payment = MapPaymentToDto(createdPayment),
            TransactionId = transactionId,
            PaymentUrl = paymentUrl
        };
    }

    private async Task<Guid> GetDefaultSalespersonIdAsync()
    {
        // Get the first active salesperson as default
        var activeSalespeople = await _salespersonRepository.GetActiveSalespeopleAsync();
        var defaultSalesperson = activeSalespeople.FirstOrDefault();
        
        if (defaultSalesperson == null)
        {
            // Create a default system salesperson if none exists
            var systemSalesperson = new Salesperson
            {
                Name = "System Sales",
                Email = "system@mainmotor.com",
                EmployeeCode = "SYS001",
                CommissionRate = 0.00m,
                IsActive = true
            };
            
            var createdSalesperson = await _salespersonRepository.AddAsync(systemSalesperson);
            return createdSalesperson.Id;
        }
        
        return defaultSalesperson.Id;
    }

    private static string? GeneratePaymentUrl(PaymentType paymentType, string transactionId)
    {
        // Mock implementation - in a real scenario, this would integrate with actual payment gateways
        return paymentType switch
        {
            PaymentType.CreditCard => $"https://payment-gateway.com/credit-card/{transactionId}",
            PaymentType.DebitCard => $"https://payment-gateway.com/debit-card/{transactionId}",
            PaymentType.BankTransfer => $"https://payment-gateway.com/bank-transfer/{transactionId}",
            PaymentType.Financing => $"https://financing-partner.com/process/{transactionId}",
            PaymentType.Cash => null, // Cash payments don't need external processing
            PaymentType.Check => null, // Check payments are processed offline
            _ => null
        };
    }

    private static PaymentDto MapPaymentToDto(Payment payment)
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

    private static SaleDto MapToDto(Sale sale)
    {
        return new SaleDto
        {
            Id = sale.Id,
            SaleDate = sale.SaleDate,
            TotalAmount = sale.TotalAmount,
            CommissionAmount = sale.CommissionAmount,
            Notes = sale.Notes,
            VehicleId = sale.VehicleId,
            CustomerId = sale.CustomerId,
            SalespersonId = sale.SalespersonId,
            CreatedAt = sale.CreatedAt,
            UpdatedAt = sale.UpdatedAt
        };
    }

    private static Sale MapToEntity(CreateSaleDto dto)
    {
        return new Sale
        {
            SaleDate = dto.SaleDate,
            TotalAmount = dto.TotalAmount,
            CommissionAmount = dto.CommissionAmount,
            Notes = dto.Notes,
            VehicleId = dto.VehicleId,
            CustomerId = dto.CustomerId,
            SalespersonId = dto.SalespersonId
        };
    }

    private static void MapToEntity(UpdateSaleDto dto, Sale entity)
    {
        entity.SaleDate = dto.SaleDate;
        entity.TotalAmount = dto.TotalAmount;
        entity.CommissionAmount = dto.CommissionAmount;
        entity.Notes = dto.Notes;
        entity.VehicleId = dto.VehicleId;
        entity.CustomerId = dto.CustomerId;
        entity.SalespersonId = dto.SalespersonId;
    }
}