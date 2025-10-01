namespace MainMotor.Application.Exceptions;

/// <summary>
/// Exception for conflict scenarios (e.g., trying to edit a sold vehicle)
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }

    public ConflictException(string message, Exception innerException) : base(message, innerException)
    {
    }
}