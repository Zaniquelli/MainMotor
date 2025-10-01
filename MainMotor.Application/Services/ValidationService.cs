using MainMotor.Application.Validators;
using MainMotor.Application.Exceptions;

namespace MainMotor.Application.Services;

/// <summary>
/// Validation service that follows Open/Closed Principle - open for extension, closed for modification
/// </summary>
public class ValidationService : IValidationService
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ValidateAsync<T>(T entity) where T : class
    {
        var validator = _serviceProvider.GetService(typeof(IBusinessRuleValidator<T>)) as IBusinessRuleValidator<T>;
        
        if (validator != null)
        {
            var result = await validator.ValidateAsync(entity);
            
            if (!result.IsValid)
            {
                var errors = result.Errors.ToDictionary(
                    error => typeof(T).Name, 
                    error => new[] { error }
                );
                
                throw new ValidationException("Business rule validation failed", errors);
            }
        }
    }
}

/// <summary>
/// Interface for validation service following Dependency Inversion Principle
/// </summary>
public interface IValidationService
{
    Task ValidateAsync<T>(T entity) where T : class;
}