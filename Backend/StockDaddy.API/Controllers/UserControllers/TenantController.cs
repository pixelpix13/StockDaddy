using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantController : ControllerBase
{
    private readonly ITenantRepository _tenantRepository;

    public TenantController(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    // GET: api/tenant
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var tenants = await _tenantRepository.GetAllAsync();
            return Ok(tenants);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching tenants: {ex.Message}");
        }
    }

    // GET: api/tenant/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var tenant = await _tenantRepository.GetByIdAsync(id);
            if (tenant == null)
                return NotFound($"Tenant with ID {id} not found.");
            return Ok(tenant);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching tenant: {ex.Message}");
        }
    }

    // POST: api/tenant
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _tenantRepository.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating tenant: {ex.Message}");
        }
    }

    // PUT: api/tenant/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTenantRequest request)
    {
        try
        {
            var existing = await _tenantRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Tenant with ID {id} not found.");

            await _tenantRepository.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating tenant: {ex.Message}");
        }
    }

    // DELETE: api/tenant/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var existing = await _tenantRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Tenant with ID {id} not found.");

            await _tenantRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting tenant: {ex.Message}");
        }
    }
}
