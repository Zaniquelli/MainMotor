using MainMotor.Application.DTOs.SeedData;
using MainMotor.Application.Interfaces;
using MainMotor.Domain.Entities;
using MainMotor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using UUIDNext;

namespace MainMotor.Infrastructure.Services;

public class DatabaseSeedingService(
    MainMotorDbContext context,
    ISeedDataReaderService seedDataReaderService,
    ILogger<DatabaseSeedingService> logger)
{
    public async Task SeedAsync()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            logger.LogInformation("Starting database seeding process with BulkInsert");

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Disable auto change detection for better performance
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            // Seed in order of dependencies
            await SeedVehicleCategoriesAsync();
            await SeedCharacteristicTypesAndCharacteristicsAsync();
            await SeedBrandsModelsAndYearsWithBulkAsync();

            stopwatch.Stop();
            logger.LogInformation("Database seeding completed successfully in {ElapsedSeconds:F2} seconds",
                stopwatch.Elapsed.TotalSeconds);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during database seeding");
            throw;
        }
        finally
        {
            // Re-enable auto change detection
            context.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }

    private async Task SeedVehicleCategoriesAsync()
    {
        if (await context.VehicleCategories.AnyAsync())
        {
            logger.LogInformation("Vehicle categories already exist, skipping seeding");
            return;
        }

        logger.LogInformation("Seeding vehicle categories");

        var categories = new List<VehicleCategory>
        {
            new() {
                Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                Name = "Cars",
                Description = "Carros",
                IsActive = true
            }
        };

        await context.AddRangeAsync(categories);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} vehicle categories", categories.Count);
    }

    private async Task SeedCharacteristicTypesAndCharacteristicsAsync()
    {
        if (await context.CharacteristicTypes.AnyAsync())
        {
            logger.LogInformation("Characteristic types already exist, skipping seeding");
            return;
        }

        logger.LogInformation("Seeding characteristic types and characteristics");

        var characteristicsData = await ReadCharacteristicsDataAsync();

        // Prepare all data before bulk insert
        var allCharacteristicTypes = new List<CharacteristicType>();
        var allCharacteristics = new List<Characteristic>();

        foreach (var typeDto in characteristicsData.CharacteristicTypes)
        {
            var characteristicType = new CharacteristicType
            {
                Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                Name = typeDto.Name,
                Description = typeDto.Description,
                IsActive = true
            };

            allCharacteristicTypes.Add(characteristicType);

            var characteristics = typeDto.Characteristics.Select(charDto => new Characteristic
            {
                Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                Name = charDto.Name,
                Description = charDto.Description,
                CharacteristicTypeId = characteristicType.Id,
                IsActive = true
            }).ToList();

            allCharacteristics.AddRange(characteristics);
        }

        await context.AddRangeAsync(allCharacteristicTypes);
        await context.AddRangeAsync(allCharacteristics);
        await context.SaveChangesAsync();
        logger.LogInformation("Completed seeding {TypeCount} characteristic types with {CharCount} characteristics",
            allCharacteristicTypes.Count, allCharacteristics.Count);
    }

    private async Task SeedBrandsModelsAndYearsWithBulkAsync()
    {
        if (await context.Brands.AnyAsync())
        {
            logger.LogInformation("Brands already exist, skipping seeding");
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        logger.LogInformation("Starting bulk seeding of brands, models, and years");

        // Load all data from JSON files
        var brandsData = await seedDataReaderService.ReadBrandModelYearDataAsync();
        var vehicleCategories = await context.VehicleCategories.ToListAsync();
        var defaultCategory = vehicleCategories.FirstOrDefault(c => c.Name == "Cars") ?? vehicleCategories[0];

        // Prepare all entities in memory
        var allBrands = new List<Brand>();
        var allModels = new List<Model>();
        var allModelYears = new List<ModelYear>();

        int brandCount = 0;
        int modelCount = 0;
        int yearCount = 0;

        foreach (var brandDto in brandsData)
        {
            var brand = new Brand
            {
                Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                Name = brandDto.Name,
                Description = brandDto.Name,
                FipeCode = brandDto.FipeCode,
                IsActive = true
            };

            allBrands.Add(brand);
            brandCount++;

            foreach (var modelDto in brandDto.Models)
            {
                var model = new Model
                {
                    Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                    Name = modelDto.Name,
                    Description = modelDto.Name,
                    FipeCode = modelDto.FipeCode,
                    BrandId = brand.Id,
                    VehicleCategoryId = defaultCategory.Id,
                    IsActive = true
                };

                allModels.Add(model);
                modelCount++;

                foreach (var yearDto in modelDto.Years)
                {
                    var modelYear = new ModelYear
                    {
                        Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
                        Year = yearDto.Year,
                        FipeCode = yearDto.FipeCode,
                        ModelId = model.Id,
                        IsActive = true
                    };

                    allModelYears.Add(modelYear);
                    yearCount++;
                }
            }

            // Progress logging
            if (brandCount % 50 == 0)
            {
                logger.LogInformation("Processed {Count} brands so far...", brandCount);
            }
        }

        logger.LogInformation("Prepared data: {Brands} brands, {Models} models, {Years} model years. Starting bulk insert...",
            brandCount, modelCount, yearCount);

        // Execute bulk inserts in order of dependencies
        var brandInsertStopwatch = Stopwatch.StartNew();
        await context.AddRangeAsync(allBrands);

        brandInsertStopwatch.Stop();
        logger.LogInformation("Inserted {Count} brands in {Seconds:F2} seconds",
            allBrands.Count, brandInsertStopwatch.Elapsed.TotalSeconds);

        var modelInsertStopwatch = Stopwatch.StartNew();
        await context.AddRangeAsync(allModels);
        modelInsertStopwatch.Stop();
        logger.LogInformation("Inserted {Count} models in {Seconds:F2} seconds",
            allModels.Count, modelInsertStopwatch.Elapsed.TotalSeconds);

        var yearInsertStopwatch = Stopwatch.StartNew();
        await context.AddRangeAsync(allModelYears);
        yearInsertStopwatch.Stop();
        logger.LogInformation("Inserted {Count} model years in {Seconds:F2} seconds",
            allModelYears.Count, yearInsertStopwatch.Elapsed.TotalSeconds);

        // Final save to commit all changes
        await context.SaveChangesAsync();
        stopwatch.Stop();
        logger.LogInformation("Completed bulk seeding in {TotalSeconds:F2} seconds: {Brands} brands, {Models} models, {Years} model years",
            stopwatch.Elapsed.TotalSeconds, brandCount, modelCount, yearCount);
    }

    public async Task<CharacteristicsSeedDataDto> ReadCharacteristicsDataAsync()
    {
        try
        {
            logger.LogInformation("Loading hardcoded characteristics data");

            var characteristicsData = new CharacteristicsSeedDataDto
            {
                CharacteristicTypes =
                [
                    new() {
                        Name = "Motor",
                        Description = "Especificações do motor",
                        Characteristics =
                        [
                            new() { Name = "1.0", Description = "Motor 1.0" },
                            new() { Name = "1.3", Description = "Motor 1.3" },
                            new() { Name = "1.4", Description = "Motor 1.4" },
                            new() { Name = "1.5", Description = "Motor 1.5" },
                            new() { Name = "1.6", Description = "Motor 1.6" },
                            new() { Name = "1.8", Description = "Motor 1.8" },
                            new() { Name = "2.0", Description = "Motor 2.0" },
                            new() { Name = "3.0", Description = "Motor 3.0" }
                        ]
                    },
                    new() {
                        Name = "Transmissão",
                        Description = "Tipos de transmissão",
                        Characteristics =
                        [
                            new() { Name = "Manual", Description = "Transmissão manual" },
                            new() { Name = "Automática", Description = "Transmissão automática" },
                            new() { Name = "CVT", Description = "Transmissão continuamente variável" },
                            new() { Name = "Automatizada", Description = "Transmissão automatizada" }
                        ]
                    },
                    new() {
                        Name = "Combustível",
                        Description = "Tipos de combustível",
                        Characteristics =
                        [
                            new() { Name = "Gasolina", Description = "Combustível gasolina" },
                            new() { Name = "Etanol", Description = "Combustível etanol" },
                            new() { Name = "Flex", Description = "Combustível flex" },
                            new() { Name = "Diesel", Description = "Combustível diesel" },
                            new() { Name = "GNV", Description = "Gás natural veicular" },
                            new() { Name = "Elétrico", Description = "Motor elétrico" },
                            new() { Name = "Hidrogênio", Description = "Motor a hidrogênio" },
                            new() { Name = "Híbrido", Description = "Motor híbrido" }
                        ]
                    },
                    new() {
                        Name = "Cor",
                        Description = "Cores disponíveis",
                        Characteristics =
                        [
                            new() { Name = "Branco", Description = "Cor branca" },
                            new() { Name = "Preto", Description = "Cor preta" },
                            new() { Name = "Prata", Description = "Cor prata" },
                            new() { Name = "Cinza", Description = "Cor cinza" },
                            new() { Name = "Azul", Description = "Cor azul" },
                            new() { Name = "Vermelho", Description = "Cor vermelha" },
                            new() { Name = "Verde", Description = "Cor verde" },
                            new() { Name = "Amarelo", Description = "Cor amarela" },
                            new() { Name = "Marrom", Description = "Cor marrom" },
                            new() { Name = "Dourado", Description = "Cor dourada" }
                        ]
                    },
                    new() {
                        Name = "Portas",
                        Description = "Número de portas",
                        Characteristics =
                        [
                            new() { Name = "2 portas", Description = "Veículo com 2 portas" },
                            new() { Name = "3 portas", Description = "Veículo com 3 portas" },
                            new() { Name = "4 portas", Description = "Veículo com 4 portas" },
                            new() { Name = "5 portas", Description = "Veículo com 5 portas" }
                        ]
                    },
                    new() {
                        Name = "Direção",
                        Description = "Tipo de direção",
                        Characteristics =
                        [
                            new() { Name = "Hidráulica", Description = "Direção hidráulica" },
                            new() { Name = "Elétrica", Description = "Direção elétrica" },
                            new() { Name = "Mecânica", Description = "Direção mecânica" },
                            new() { Name = "Eletro-hidráulica", Description = "Direção eletro-hidráulica" }
                        ]
                    }
                ]
            };

            logger.LogInformation("Successfully loaded {Count} characteristic types with hardcoded data",
                characteristicsData.CharacteristicTypes.Count);

            return await Task.FromResult(characteristicsData);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading hardcoded characteristics data");
            throw new InvalidOperationException($"Failed to load characteristics data: {ex.Message}", ex);
        }
    }
}