using MainMotor.Domain.Enums;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MainMotor.API.Filters;

/// <summary>
/// Schema filter to add descriptions and examples for enums in Swagger documentation
/// </summary>
public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();

            var enumValues = new List<IOpenApiAny>();
            var enumDescriptions = new List<string>();

            foreach (var enumValue in Enum.GetValues(context.Type))
            {
                var enumName = Enum.GetName(context.Type, enumValue);
                var enumInt = (int)enumValue;

                enumValues.Add(new OpenApiInteger(enumInt));

                // Add descriptions for specific enums
                var description = GetEnumDescription(context.Type, enumName, enumInt);
                enumDescriptions.Add($"{enumInt} = {enumName}: {description}");
            }

            schema.Enum = enumValues;
            schema.Description = string.Join(", ", enumDescriptions);
            schema.Type = "integer";
            schema.Format = "int32";
        }
    }

    private static string GetEnumDescription(Type enumType, string? enumName, int enumValue)
    {
        return enumType.Name switch
        {
            nameof(VehicleStatus) => enumValue switch
            {
                1 => "Vehicle is available for sale",
                2 => "Vehicle is reserved (sale in progress)",
                3 => "Vehicle has been sold",
                4 => "Vehicle is in maintenance",
                5 => "Vehicle is temporarily unavailable",
                _ => "Unknown status"
            },
            nameof(PaymentStatus) => enumValue switch
            {
                1 => "Payment is pending processing",
                2 => "Payment has been completed successfully",
                3 => "Payment has failed",
                4 => "Payment has been cancelled",
                5 => "Payment has been refunded",
                _ => "Unknown status"
            },
            nameof(PaymentType) => enumValue switch
            {
                1 => "Cash payment",
                2 => "Credit card payment",
                3 => "Debit card payment",
                4 => "Bank transfer",
                5 => "Financing",
                6 => "Check payment",
                _ => "Unknown payment type"
            },
            _ => enumName ?? "Unknown"
        };
    }
}