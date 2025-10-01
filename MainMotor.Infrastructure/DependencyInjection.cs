using MainMotor.Application.Interfaces;
using MainMotor.Domain.Repositories;
using MainMotor.Infrastructure.Data;
using MainMotor.Infrastructure.Repositories;
using MainMotor.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MainMotor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<MainMotorDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), b =>
                b.MigrationsAssembly(typeof(MainMotorDbContext).Assembly.FullName)
            )
        );

        // Register repositories
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ISalespersonRepository, SalespersonRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IModelYearRepository, ModelYearRepository>();
        services.AddScoped<ICharacteristicRepository, CharacteristicRepository>();
        services.AddScoped<ICharacteristicTypeRepository, CharacteristicTypeRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IModelRepository, ModelRepository>();

        // Register services
        services.AddScoped<ISeedDataReaderService, SeedDataReaderService>();
        services.AddScoped<DatabaseSeedingService>();

        return services;
    }
}