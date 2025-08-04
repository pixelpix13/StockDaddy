using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerRepository _customerRepo;

    public CustomerController(ICustomerRepository customerRepo)
    {
        _customerRepo = customerRepo;
    }

    // GET: api/customer
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var customers = await _customerRepo.GetAllAsync();
            return Ok(customers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving customers: {ex.Message}");
        }
    }

    // GET: api/customer/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var customer = await _customerRepo.GetByIdAsync(id);
            if (customer == null)
                return NotFound($"Customer with ID {id} not found.");

            return Ok(customer);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching customer: {ex.Message}");
        }
    }

    // POST: api/customer
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Customer customer)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _customerRepo.AddAsync(customer);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating customer: {ex.Message}");
        }
    }

    // PUT: api/customer/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Customer customer)
    {
        try
        {
            if (id != customer.Id)
                return BadRequest("ID in URL and body must match.");

            var existing = await _customerRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Customer with ID {id} not found.");

            await _customerRepo.UpdateAsync(customer);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating customer: {ex.Message}");
        }
    }

    // DELETE: api/customer/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDelete(Guid id)
    {
        try
        {
            var existing = await _customerRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Customer with ID {id} not found.");

            await _customerRepo.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting customer: {ex.Message}");
        }
    }
}
