using MainMotor.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MainMotor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModelYearsController : ControllerBase
{
    private readonly MainMotorDbContext _context;

    public ModelYearsController(MainMotorDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all active model years
    /// </summary>
    /// <returns>List of all active model years with brand and model information</returns>
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var modelYears = await _context.ModelYears
            .Include(my => my.Model)
                .ThenInclude(m => m.Brand)
            .Where(my => my.IsActive)
            .Select(my => new
            {
                my.Id,
                my.Year,
                my.ModelId,
                ModelName = my.Model.Name,
                BrandId = my.Model.BrandId,
                BrandName = my.Model.Brand.Name,
                my.FipeCode,
                my.IsActive
            })
            .OrderBy(my => my.BrandName)
            .ThenBy(my => my.ModelName)
            .ThenBy(my => my.Year)
            .ToListAsync();

        return Ok(modelYears);
    }

    /// <summary>
    /// Get model years by model
    /// </summary>
    /// <param name="modelId">Unique identifier of the model</param>
    /// <returns>List of model years for the specified model</returns>
    [HttpGet("by-model/{modelId}")]
    public async Task<ActionResult> GetByModel(Guid modelId)
    {
        var modelYears = await _context.ModelYears
            .Include(my => my.Model)
                .ThenInclude(m => m.Brand)
            .Where(my => my.ModelId == modelId && my.IsActive)
            .Select(my => new
            {
                my.Id,
                my.Year,
                my.ModelId,
                ModelName = my.Model.Name,
                BrandId = my.Model.BrandId,
                BrandName = my.Model.Brand.Name,
                my.FipeCode,
                my.IsActive
            })
            .OrderBy(my => my.Year)
            .ToListAsync();

        return Ok(modelYears);
    }
}