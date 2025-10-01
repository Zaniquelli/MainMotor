using MainMotor.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MainMotor.Application.DTOs;

/// <summary>
/// Vehicle data transfer object containing complete vehicle information
/// </summary>
/// <remarks>
/// Includes marketplace-specific properties like brand name, model name, year, and sale date.
/// Used for displaying vehicle information in listings and detailed views.
/// </remarks>
public class VehicleDto
{
    /// <summary>Unique identifier for the vehicle</summary>
    public Guid Id { get; set; }

    /// <summary>Vehicle Identification Number (11 characters)</summary>
    public string VinNumber { get; set; } = string.Empty;

    /// <summary>Vehicle license plate number</summary>
    public string LicensePlate { get; set; } = string.Empty;

    /// <summary>Vehicle mileage in kilometers</summary>
    public int Mileage { get; set; }

    /// <summary>Price paid when purchasing the vehicle</summary>
    public decimal PurchasePrice { get; set; }

    /// <summary>Current sale price for the vehicle</summary>
    public decimal SalePrice { get; set; }

    /// <summary>Current status of the vehicle (1=Available, 2=Reserved, 3=Sold, 4=InMaintenance, 5=Unavailable)</summary>
    public VehicleStatus Status { get; set; }

    /// <summary>Additional notes about the vehicle</summary>
    public string? Notes { get; set; }

    /// <summary>Reference to the model year (links to brand/model/year information)</summary>
    public Guid ModelYearId { get; set; }

    /// <summary>Date and time when the vehicle was created in the system</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Date and time when the vehicle was last updated</summary>
    public DateTime? UpdatedAt { get; set; }

    // Marketplace-specific properties
    /// <summary>Brand name (e.g., Toyota, Honda, Ford)</summary>
    public string BrandName { get; set; } = string.Empty;

    /// <summary>Model name (e.g., Corolla, Civic, Focus)</summary>
    public string ModelName { get; set; } = string.Empty;

    /// <summary>Model year</summary>
    public int Year { get; set; }

    /// <summary>Date when the vehicle was sold (only populated for sold vehicles)</summary>
    public DateTime? SaleDate { get; set; }

    /// <summary>Collection of characteristics associated with this vehicle</summary>
    public ICollection<VehicleCharacteristicDto> Characteristics { get; set; } = new List<VehicleCharacteristicDto>();
}

/// <summary>
/// Data transfer object for creating a new vehicle
/// </summary>
/// <remarks>
/// Contains all required information for vehicle registration including validation rules.
/// VIN number must be unique across all vehicles in the system.
/// </remarks>
public class CreateVehicleDto
{
    /// <summary>
    /// Vehicle Identification Number - must be exactly 17 characters, valid VIN format
    /// </summary>
    /// <example>1HGBH41JXMN109186</example>
    [Required(ErrorMessage = "VIN number is required")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "VIN number must be exactly 11 characters")]
    public string VinNumber { get; set; } = string.Empty;

    /// <summary>
    /// Vehicle license plate number - must be between 7 and 10 characters
    /// </summary>
    /// <example>ABC1234</example>
    [Required(ErrorMessage = "License plate is required")]
    [StringLength(10, MinimumLength = 7, ErrorMessage = "License plate must be between 7 and 10 characters")]
    public string LicensePlate { get; set; } = string.Empty;

    /// <summary>
    /// Vehicle mileage in kilometers - must be between 0 and 999,999
    /// </summary>
    /// <example>50000</example>
    [Range(0, 999999, ErrorMessage = "Mileage must be between 0 and 999,999")]
    public int Mileage { get; set; }

    /// <summary>
    /// Price paid when purchasing the vehicle - must be greater than 0
    /// </summary>
    /// <example>45000.00</example>
    [Range(0.01, double.MaxValue, ErrorMessage = "Purchase price must be greater than 0")]
    public decimal PurchasePrice { get; set; }

    /// <summary>
    /// Desired sale price for the vehicle - must be greater than 0
    /// </summary>
    /// <example>52000.00</example>
    [Range(0.01, double.MaxValue, ErrorMessage = "Sale price must be greater than 0")]
    public decimal SalePrice { get; set; }

    /// <summary>
    /// Initial status of the vehicle - defaults to Available (1)
    /// </summary>
    /// <example>1</example>
    public VehicleStatus Status { get; set; } = VehicleStatus.Available;

    /// <summary>
    /// Additional notes about the vehicle - maximum 500 characters
    /// </summary>
    /// <example>Veículo em excelente estado</example>
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }

    /// <summary>
    /// Reference to the model year that defines the brand, model, and year
    /// </summary>
    /// <example>123e4567-e89b-12d3-a456-426614174000</example>
    [Required(ErrorMessage = "Model year ID is required")]
    public Guid ModelYearId { get; set; }

    /// <summary>
    /// Collection of characteristic IDs to be associated with this vehicle
    /// </summary>
    /// <example>["123e4567-e89b-12d3-a456-426614174001", "123e4567-e89b-12d3-a456-426614174002"]</example>
    [Required(ErrorMessage = "At least one characteristic is required")]
    [MinLength(1, ErrorMessage = "At least one characteristic must be selected")]
    public ICollection<Guid> CharacteristicIds { get; set; } = new List<Guid>();
}

public class UpdateVehicleDto
{
    [Required(ErrorMessage = "VIN number is required")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "VIN number must be exactly 11 characters")]
    public string VinNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "License plate is required")]
    [StringLength(10, MinimumLength = 7, ErrorMessage = "License plate must be between 7 and 10 characters")]
    public string LicensePlate { get; set; } = string.Empty;

    [Range(0, 999999, ErrorMessage = "Mileage must be between 0 and 999,999")]
    public int Mileage { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Purchase price must be greater than 0")]
    public decimal PurchasePrice { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Sale price must be greater than 0")]
    public decimal SalePrice { get; set; }

    public VehicleStatus Status { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }

    [Required(ErrorMessage = "Model year ID is required")]
    public Guid ModelYearId { get; set; }

    /// <summary>
    /// Collection of characteristic IDs to be associated with this vehicle
    /// </summary>
    /// <example>["123e4567-e89b-12d3-a456-426614174001", "123e4567-e89b-12d3-a456-426614174002"]</example>
    [Required(ErrorMessage = "At least one characteristic is required")]
    [MinLength(1, ErrorMessage = "At least one characteristic must be selected")]
    public ICollection<Guid> CharacteristicIds { get; set; } = new List<Guid>();
}