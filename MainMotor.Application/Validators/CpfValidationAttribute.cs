using DocumentValidator;
using System.ComponentModel.DataAnnotations;

namespace MainMotor.Application.Validators;

/// <summary>
/// Custom validation attribute for CPF validation
/// </summary>
public class CpfValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string cpf)
        {
            return false;
        }

        return CpfValidation.Validate(cpf);
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The {name} field contains an invalid CPF number.";
    }
}