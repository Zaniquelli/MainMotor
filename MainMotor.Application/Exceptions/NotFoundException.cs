namespace MainMotor.Application.Exceptions;

/// <summary>
/// Exception for when a requested resource is not found
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entityName, object key) 
        : base($"{entityName} with key '{key}' was not found.")
    {
    }
}