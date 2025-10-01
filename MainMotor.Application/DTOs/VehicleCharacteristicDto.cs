namespace MainMotor.Application.DTOs;

/// <summary>
/// Data transfer object for vehicle characteristics
/// </summary>
/// <remarks>
/// Contains characteristic information associated with a vehicle including type information.
/// Used for displaying vehicle characteristics in vehicle details.
/// </remarks>
public class VehicleCharacteristicDto
{
    /// <summary>Unique identifier for the characteristic</summary>
    public Guid CharacteristicId { get; set; }

    /// <summary>Name of the characteristic (e.g., "Red", "Automatic", "Leather")</summary>
    public string CharacteristicName { get; set; } = string.Empty;

    /// <summary>Name of the characteristic type (e.g., "Color", "Transmission", "Interior")</summary>
    public string CharacteristicTypeName { get; set; } = string.Empty;
}