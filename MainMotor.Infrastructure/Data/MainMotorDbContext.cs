using MainMotor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MainMotor.Infrastructure.Data;

public class MainMotorDbContext : DbContext
{
    public MainMotorDbContext(DbContextOptions<MainMotorDbContext> options) : base(options)
    {
    }

    // Core entities
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Salesperson> Salespeople { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<Payment> Payments { get; set; }

    // Reference entities
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Model> Models { get; set; }
    public DbSet<ModelYear> ModelYears { get; set; }
    public DbSet<VehicleCategory> VehicleCategories { get; set; }
    public DbSet<CharacteristicType> CharacteristicTypes { get; set; }
    public DbSet<Characteristic> Characteristics { get; set; }
    public DbSet<FipePriceHistory> FipePriceHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure entity mappings
        ConfigureVehicle(modelBuilder);
        ConfigureCustomer(modelBuilder);
        ConfigureSalesperson(modelBuilder);
        ConfigureSale(modelBuilder);
        ConfigurePayment(modelBuilder);
        ConfigureReferenceEntities(modelBuilder);
    }

    private static void ConfigureVehicle(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VinNumber).HasMaxLength(17).IsRequired();
            entity.Property(e => e.LicensePlate).HasMaxLength(10);
            entity.Property(e => e.PurchasePrice).HasPrecision(18, 2);
            entity.Property(e => e.SalePrice).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            // Relationships
            entity.HasOne(e => e.ModelYear)
                  .WithMany(m => m.Vehicles)
                  .HasForeignKey(e => e.ModelYearId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Sales)
                  .WithOne(s => s.Vehicle)
                  .HasForeignKey(s => s.VehicleId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Many-to-many relationship with Characteristics
            entity.HasMany(e => e.Characteristics)
                  .WithMany()
                  .UsingEntity(j => j.ToTable("VehicleCharacteristics"));
        });
    }

    private static void ConfigureCustomer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Document).HasMaxLength(20);

            // Relationships
            entity.HasMany(e => e.Sales)
                  .WithOne(s => s.Customer)
                  .HasForeignKey(s => s.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureSalesperson(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Salesperson>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.CommissionRate).HasPrecision(5, 4);

            // Relationships
            entity.HasMany(e => e.Sales)
                  .WithOne(s => s.Salesperson)
                  .HasForeignKey(s => s.SalespersonId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureSale(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.CommissionAmount).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            // Relationships
            entity.HasOne(e => e.Vehicle)
                  .WithMany(v => v.Sales)
                  .HasForeignKey(e => e.VehicleId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Sales)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Salesperson)
                  .WithMany(s => s.Sales)
                  .HasForeignKey(e => e.SalespersonId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Payments)
                  .WithOne(p => p.Sale)
                  .HasForeignKey(p => p.SaleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigurePayment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(500);

            // Relationships
            entity.HasOne(e => e.Sale)
                  .WithMany(s => s.Payments)
                  .HasForeignKey(e => e.SaleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureReferenceEntities(ModelBuilder modelBuilder)
    {
        // Brand configuration
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.FipeCode).HasMaxLength(20).IsRequired();

            entity.HasMany(e => e.Models)
                  .WithOne(m => m.Brand)
                  .HasForeignKey(m => m.BrandId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Model configuration
        modelBuilder.Entity<Model>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.FipeCode).HasMaxLength(20).IsRequired();

            entity.HasOne(e => e.Brand)
                  .WithMany(b => b.Models)
                  .HasForeignKey(e => e.BrandId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.VehicleCategory)
                  .WithMany(c => c.Models)
                  .HasForeignKey(e => e.VehicleCategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.ModelYears)
                  .WithOne(my => my.Model)
                  .HasForeignKey(my => my.ModelId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ModelYear configuration
        modelBuilder.Entity<ModelYear>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Year).IsRequired();
            entity.Property(e => e.FipeCode).HasMaxLength(20).IsRequired();

            entity.HasOne(e => e.Model)
                  .WithMany(m => m.ModelYears)
                  .HasForeignKey(e => e.ModelId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Vehicles)
                  .WithOne(v => v.ModelYear)
                  .HasForeignKey(v => v.ModelYearId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.FipePriceHistories)
                  .WithOne(f => f.ModelYear)
                  .HasForeignKey(f => f.ModelYearId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // VehicleCategory configuration
        modelBuilder.Entity<VehicleCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();

            entity.HasMany(e => e.Models)
                  .WithOne(m => m.VehicleCategory)
                  .HasForeignKey(m => m.VehicleCategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // CharacteristicType configuration
        modelBuilder.Entity<CharacteristicType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();

            entity.HasMany(e => e.Characteristics)
                  .WithOne(c => c.CharacteristicType)
                  .HasForeignKey(c => c.CharacteristicTypeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Characteristic configuration
        modelBuilder.Entity<Characteristic>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();

            entity.HasOne(e => e.CharacteristicType)
                  .WithMany(ct => ct.Characteristics)
                  .HasForeignKey(e => e.CharacteristicTypeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // FipePriceHistory configuration
        modelBuilder.Entity<FipePriceHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasPrecision(18, 2).IsRequired();

            entity.HasOne(e => e.ModelYear)
                  .WithMany(my => my.FipePriceHistories)
                  .HasForeignKey(e => e.ModelYearId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}