using MainMotor.API.Filters;
using MainMotor.API.Middleware;
using MainMotor.Application;
using MainMotor.Infrastructure;
using MainMotor.Infrastructure.Data;
using MainMotor.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    // Add global model validation filter
    options.Filters.Add<ModelValidationFilter>();
});

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "MainMotor API",
        Version = "v1",
        Description = "API para plataforma online de revenda de veículos desenvolvida em .NET 9 com Clean Architecture. " +
                     "Oferece funcionalidades completas para cadastro, edição, venda e listagem de veículos"
    });

    // Include XML comments for better documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Add enum descriptions
    c.SchemaFilter<EnumSchemaFilter>();

    // Add examples for request/response models
    c.SchemaFilter<ExampleSchemaFilter>();

    // Configure authorization if needed in the future
    //c.AddSecurityDefinition("Bearer", new()
    //{
    //    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
    //    Name = "Authorization",
    //    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
    //    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
    //    Scheme = "Bearer"
    //});
});

// Add Application services
builder.Services.AddApplication();

// Add Infrastructure services (includes EF Core configuration)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
// Add global exception handling middleware
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MainMotor API v1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MainMotorDbContext>();
    var seedingService = scope.ServiceProvider.GetRequiredService<DatabaseSeedingService>();

    // Apply any pending migrations
    await context.Database.MigrateAsync();

    // Seed reference data using the new service
    await seedingService.SeedAsync();
}

await app.RunAsync();
