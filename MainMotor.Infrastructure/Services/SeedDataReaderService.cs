using MainMotor.Application.DTOs.SeedData;
using MainMotor.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MainMotor.Infrastructure.Services;

public class SeedDataReaderService(ILogger<SeedDataReaderService> logger) : ISeedDataReaderService
{
    private readonly string _dataDirectory = GetDataDirectory();

    private static string GetDataDirectory()
    {
        // Try container path first (when running in Docker)
        var containerPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Brands-Models-Years-FIPE");
        if (Directory.Exists(containerPath))
        {
            return containerPath;
        }

        // Fallback to development path (when running with dotnet run)
        var developmentPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "MainMotor.Infrastructure", "Data", "Brands-Models-Years-FIPE");
        if (Directory.Exists(developmentPath))
        {
            return developmentPath;
        }

        // Last resort: try relative to assembly location
        var assemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        var assemblyRelativePath = Path.Combine(assemblyPath!, "Data", "Brands-Models-Years-FIPE");

        return assemblyRelativePath;
    }

    public async Task<IEnumerable<BrandSeedDataDto>> ReadBrandModelYearDataAsync()
    {
        var brands = new List<BrandSeedDataDto>();

        try
        {
            if (!Directory.Exists(_dataDirectory))
            {
                logger.LogWarning("Brands-Models-Years-FIPE directory not found at: {Directory}", _dataDirectory);
                return brands;
            }

            var jsonFiles = Directory.GetFiles(_dataDirectory, "*.json", SearchOption.AllDirectories);
            logger.LogInformation("Found {Count} JSON files in {Directory}", jsonFiles.Length, _dataDirectory);

            foreach (var filePath in jsonFiles)
            {
                try
                {
                    var brandData = await ReadBrandFileAsync(filePath);
                    if (brandData != null)
                    {
                        brands.Add(brandData);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error reading brand file: {FilePath}", filePath);
                    // Continue processing other files even if one fails
                }
            }

            logger.LogInformation("Successfully processed {Count} brand files", brands.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error reading brand model year data from directory: {Directory}", _dataDirectory);
            throw new InvalidOperationException($"Failed to read brand model year data: {ex.Message}", ex);
        }

        return brands;
    }

    private async Task<BrandSeedDataDto?> ReadBrandFileAsync(string filePath)
    {
        try
        {
            var jsonContent = await File.ReadAllTextAsync(filePath);

            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                logger.LogWarning("Brand file is empty: {FilePath}", filePath);
                return null;
            }

            // Parse the JSON structure based on expected FIPE data format
            var jsonDocument = JsonDocument.Parse(jsonContent);
            var brandData = ParseBrandFromJson(jsonDocument, filePath);

            if (brandData != null)
            {
                logger.LogDebug("Successfully parsed brand: {BrandName} with {ModelCount} models from {FilePath}",
                    brandData.Name, brandData.Models.Count, filePath);
            }

            return brandData;
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "JSON parsing error in brand file: {FilePath}", filePath);
            throw new InvalidOperationException($"Invalid JSON format in file {filePath}: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error reading brand file: {FilePath}", filePath);
            throw;
        }
    }

    private BrandSeedDataDto? ParseBrandFromJson(JsonDocument jsonDocument, string filePath)
    {
        try
        {
            var root = jsonDocument.RootElement;

            // Extract brand information - this will need to be adjusted based on actual JSON structure
            var brandName = ExtractBrandName(root, filePath);
            var brandFipeCode = ExtractBrandFipeCode(root);

            if (string.IsNullOrEmpty(brandName))
            {
                logger.LogWarning("Could not extract brand name from file: {FilePath}", filePath);
                return null;
            }

            var brand = new BrandSeedDataDto
            {
                Name = brandName,
                FipeCode = brandFipeCode,
                Models = ParseModelsFromJson(root)
            };

            return brand;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error parsing brand JSON structure from file: {FilePath}", filePath);
            throw new InvalidOperationException($"Failed to parse brand data from {filePath}: {ex.Message}", ex);
        }
    }

    private static string ExtractBrandName(JsonElement root, string filePath)
    {
        // Try different possible JSON structures for brand name
        if (root.TryGetProperty("name", out var nameElement) && nameElement.ValueKind == JsonValueKind.String)
        {
            return nameElement.GetString() ?? string.Empty;
        }

        if (root.TryGetProperty("brand", out var brandElement) && brandElement.ValueKind == JsonValueKind.String)
        {
            return brandElement.GetString() ?? string.Empty;
        }

        // Fallback: extract from filename (remove "cars_" prefix)
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        if (fileName.StartsWith("cars_"))
        {
            fileName = fileName[5..]; // Remove "cars_" prefix
        }
        return fileName.Replace("-", " ").Replace("_", " ");
    }

    private static string ExtractBrandFipeCode(JsonElement root)
    {
        if (root.TryGetProperty("code", out var codeElement) && codeElement.ValueKind == JsonValueKind.String)
        {
            return codeElement.GetString() ?? string.Empty;
        }

        if (root.TryGetProperty("fipeCode", out var fipeElement) && fipeElement.ValueKind == JsonValueKind.String)
        {
            return fipeElement.GetString() ?? string.Empty;
        }

        return string.Empty;
    }

    private List<ModelSeedDataDto> ParseModelsFromJson(JsonElement root)
    {
        var models = new List<ModelSeedDataDto>();

        try
        {
            // Try different possible JSON structures for models
            if (root.TryGetProperty("models", out var modelsElement) && modelsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var modelElement in modelsElement.EnumerateArray())
                {
                    var model = ParseModelFromElement(modelElement);
                    if (model != null)
                    {
                        models.Add(model);
                    }
                }
            }
            else if (root.ValueKind == JsonValueKind.Array)
            {
                // If root is an array of models
                foreach (var modelElement in root.EnumerateArray())
                {
                    var model = ParseModelFromElement(modelElement);
                    if (model != null)
                    {
                        models.Add(model);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error parsing models from JSON");
            // Return empty list instead of throwing to allow partial data processing
        }

        return models;
    }

    private ModelSeedDataDto? ParseModelFromElement(JsonElement modelElement)
    {
        try
        {
            var modelName = string.Empty;
            var modelFipeCode = string.Empty;

            if (modelElement.TryGetProperty("name", out var nameElement) && nameElement.ValueKind == JsonValueKind.String)
            {
                modelName = nameElement.GetString() ?? string.Empty;
            }

            if (modelElement.TryGetProperty("code", out var codeElement) && codeElement.ValueKind == JsonValueKind.String)
            {
                modelFipeCode = codeElement.GetString() ?? string.Empty;
            }

            if (string.IsNullOrEmpty(modelName))
            {
                return null;
            }

            var model = new ModelSeedDataDto
            {
                Name = modelName,
                FipeCode = modelFipeCode,
                Years = ParseModelYearsFromElement(modelElement)
            };

            return model;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error parsing model from JSON element");
            return null;
        }
    }

    private List<ModelYearSeedDataDto> ParseModelYearsFromElement(JsonElement modelElement)
    {
        var years = new List<ModelYearSeedDataDto>();

        try
        {
            if (modelElement.TryGetProperty("years", out var yearsElement) && yearsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var yearElement in yearsElement.EnumerateArray())
                {
                    var year = ParseModelYearFromElement(yearElement);
                    if (year != null)
                    {
                        years.Add(year);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error parsing model years from JSON element");
            // Return empty list instead of throwing to allow partial data processing
        }

        return years;
    }

    private ModelYearSeedDataDto? ParseModelYearFromElement(JsonElement yearElement)
    {
        try
        {
            var year = 0;
            var fipeCode = string.Empty;
            var fuelType = string.Empty;

            // Parse from "name" field which contains format like "1995 Gasolina"
            if (yearElement.TryGetProperty("name", out var nameElement) && nameElement.ValueKind == JsonValueKind.String)
            {
                var nameValue = nameElement.GetString() ?? string.Empty;
                var parts = nameValue.Split(' ');

                if (parts.Length >= 1 && int.TryParse(parts[0], out var parsedYear))
                {
                    year = parsedYear;
                }

                if (parts.Length >= 2)
                {
                    fuelType = parts[1];
                }
            }

            if (yearElement.TryGetProperty("code", out var codeElement) && codeElement.ValueKind == JsonValueKind.String)
            {
                fipeCode = codeElement.GetString() ?? string.Empty;
            }

            if (year == 0)
            {
                return null;
            }

            return new ModelYearSeedDataDto
            {
                Year = year,
                FipeCode = fipeCode,
                FuelType = fuelType
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error parsing model year from JSON element");
            return null;
        }
    }
}