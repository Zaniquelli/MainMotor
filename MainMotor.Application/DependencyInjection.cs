using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;
using MainMotor.Application.Services;
using MainMotor.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace MainMotor.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISalespersonService, SalespersonService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<ICharacteristicService, CharacteristicService>();
        services.AddScoped<ICharacteristicTypeService, CharacteristicTypeService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<IModelService, ModelService>();

        // Register validation services
        services.AddScoped<IValidationService, ValidationService>();
        services.AddScoped<IBusinessRuleValidator<CreateVehicleDto>, VehicleBusinessRuleValidator>();

        return services;
    }
}