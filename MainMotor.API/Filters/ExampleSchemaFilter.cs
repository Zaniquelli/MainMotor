using MainMotor.Application.DTOs;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MainMotor.API.Filters;

/// <summary>
/// Schema filter to add examples for DTOs in Swagger documentation
/// </summary>
public class ExampleSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(CreateVehicleDto))
        {
            schema.Example = new OpenApiObject
            {
                ["vinNumber"] = new OpenApiString("1HGBH41JXMN109186"),
                ["licensePlate"] = new OpenApiString("ABC1234"),
                ["color"] = new OpenApiString("Preto"),
                ["mileage"] = new OpenApiInteger(50000),
                ["purchasePrice"] = new OpenApiDouble(45000.00),
                ["salePrice"] = new OpenApiDouble(52000.00),
                ["status"] = new OpenApiInteger(1),
                ["notes"] = new OpenApiString("Veículo em excelente estado"),
                ["modelYearId"] = new OpenApiString("123e4567-e89b-12d3-a456-426614174000")
            };
        }
        else if (context.Type == typeof(UpdateVehicleDto))
        {
            schema.Example = new OpenApiObject
            {
                ["vinNumber"] = new OpenApiString("1HGBH41JXMN109186"),
                ["licensePlate"] = new OpenApiString("ABC1234"),
                ["color"] = new OpenApiString("Azul"),
                ["mileage"] = new OpenApiInteger(52000),
                ["purchasePrice"] = new OpenApiDouble(45000.00),
                ["salePrice"] = new OpenApiDouble(50000.00),
                ["status"] = new OpenApiInteger(1),
                ["notes"] = new OpenApiString("Preço reduzido para venda rápida"),
                ["modelYearId"] = new OpenApiString("123e4567-e89b-12d3-a456-426614174000")
            };
        }
        else if (context.Type == typeof(RegisterSaleDto))
        {
            schema.Example = new OpenApiObject
            {
                ["customerCpf"] = new OpenApiString("12345678901"),
                ["saleDate"] = new OpenApiString("2024-01-15T10:30:00Z"),
                ["vehicleId"] = new OpenApiString("123e4567-e89b-12d3-a456-426614174000"),
                ["customerName"] = new OpenApiString("João Silva"),
                ["customerEmail"] = new OpenApiString("joao.silva@email.com"),
                ["customerPhone"] = new OpenApiString("(11) 99999-9999"),
                ["paymentType"] = new OpenApiInteger(2)
            };
        }
        else if (context.Type == typeof(RegisterSaleResponseDto))
        {
            schema.Example = new OpenApiObject
            {
                ["sale"] = new OpenApiObject
                {
                    ["id"] = new OpenApiString("123e4567-e89b-12d3-a456-426614174000"),
                    ["saleDate"] = new OpenApiString("2024-01-15T10:30:00Z"),
                    ["totalAmount"] = new OpenApiDouble(52000.00),
                    ["commissionAmount"] = new OpenApiDouble(0.00),
                    ["notes"] = new OpenApiString("Sale registered via marketplace"),
                    ["vehicleId"] = new OpenApiString("123e4567-e89b-12d3-a456-426614174000"),
                    ["customerId"] = new OpenApiString("456e7890-e89b-12d3-a456-426614174000"),
                    ["salespersonId"] = new OpenApiString("789e0123-e89b-12d3-a456-426614174000"),
                    ["createdAt"] = new OpenApiString("2024-01-15T10:30:00Z"),
                    ["updatedAt"] = new OpenApiNull()
                },
                ["payment"] = new OpenApiObject
                {
                    ["id"] = new OpenApiString("abc1234-e89b-12d3-a456-426614174000"),
                    ["amount"] = new OpenApiDouble(52000.00),
                    ["paymentDate"] = new OpenApiString("2024-01-15T10:30:00Z"),
                    ["paymentType"] = new OpenApiInteger(2),
                    ["status"] = new OpenApiInteger(1),
                    ["transactionId"] = new OpenApiString("PAY_123e4567e89b12d3a456426614174000"),
                    ["notes"] = new OpenApiString("Payment pending for marketplace sale - CreditCard"),
                    ["saleId"] = new OpenApiString("123e4567-e89b-12d3-a456-426614174000"),
                    ["createdAt"] = new OpenApiString("2024-01-15T10:30:00Z"),
                    ["updatedAt"] = new OpenApiNull()
                },
                ["transactionId"] = new OpenApiString("PAY_123e4567e89b12d3a456426614174000"),
                ["paymentUrl"] = new OpenApiString("https://payment-gateway.com/credit-card/PAY_123e4567e89b12d3a456426614174000")
            };
        }
        else if (context.Type == typeof(PaymentWebhookDto))
        {
            schema.Example = new OpenApiObject
            {
                ["transactionId"] = new OpenApiString("TXN123456789"),
                ["status"] = new OpenApiString("paid"),
                ["saleId"] = new OpenApiString("123e4567-e89b-12d3-a456-426614174000")
            };
        }
        else if (context.Type == typeof(CreateCustomerDto))
        {
            schema.Example = new OpenApiObject
            {
                ["name"] = new OpenApiString("Maria Santos"),
                ["email"] = new OpenApiString("maria.santos@email.com"),
                ["phone"] = new OpenApiString("(11) 98888-8888"),
                ["address"] = new OpenApiString("Rua das Flores, 123 - São Paulo, SP"),
                ["document"] = new OpenApiString("98765432100"),
                ["isActive"] = new OpenApiBoolean(true)
            };
        }
        else if (context.Type == typeof(CreatePaymentDto))
        {
            schema.Example = new OpenApiObject
            {
                ["amount"] = new OpenApiDouble(52000.00),
                ["paymentDate"] = new OpenApiString("2024-01-15T14:30:00Z"),
                ["paymentType"] = new OpenApiInteger(2),
                ["status"] = new OpenApiInteger(1),
                ["transactionId"] = new OpenApiString("TXN123456789"),
                ["notes"] = new OpenApiString("Pagamento via cartão de crédito"),
                ["saleId"] = new OpenApiString("123e4567-e89b-12d3-a456-426614174000")
            };
        }
    }
}