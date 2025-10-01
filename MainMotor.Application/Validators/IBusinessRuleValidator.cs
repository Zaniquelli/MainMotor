namespace MainMotor.Application.Validators;

/// <summary>
/// Interface for business rule validation following Single Responsibility Principle
/// </summary>
/// <typeparam name="T">The type to validate</typeparam>
public interface IBusinessRuleValidator<T>
{
    Task<ValidationResult> ValidateAsync(T entity);
}

/// <summary>
/// Result of business rule validation
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ValidationResult Success() => new() { IsValid = true };
    
    public static ValidationResult Failure(params string[] errors) => new() 
    { 
        IsValid = false, 
        Errors = errors.ToList() 
    };
}