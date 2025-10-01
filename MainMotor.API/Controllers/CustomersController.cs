using MainMotor.Application.DTOs;
using MainMotor.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MainMotor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Get all customers
    /// </summary>
    /// <returns>List of customers</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }

    /// <summary>
    /// Get customer by ID
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>Customer details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }
        return Ok(customer);
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    /// <param name="createCustomerDto">Customer data</param>
    /// <returns>Created customer</returns>
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(CreateCustomerDto createCustomerDto)
    {
        var customer = await _customerService.CreateAsync(createCustomerDto);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    /// <summary>
    /// Update an existing customer
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="updateCustomerDto">Updated customer data</param>
    /// <returns>Updated customer</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerDto>> Update(Guid id, UpdateCustomerDto updateCustomerDto)
    {
        var customer = await _customerService.UpdateAsync(id, updateCustomerDto);
        if (customer == null)
        {
            return NotFound();
        }
        return Ok(customer);
    }

    /// <summary>
    /// Delete a customer
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _customerService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}