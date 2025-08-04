using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductRestockAlertController : ControllerBase
{
    private readonly IProductRestockAlertRepository _alertRepository;

    public ProductRestockAlertController(IProductRestockAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    // GET: api/productrestockalert
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var alerts = await _alertRepository.GetAllAsync();
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching alerts: {ex.Message}");
        }
    }

    // GET: api/productrestockalert/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var alert = await _alertRepository.GetByIdAsync(id);
            if (alert == null)
                return NotFound($"Alert with ID {id} not found.");

            return Ok(alert);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving alert: {ex.Message}");
        }
    }

    // POST: api/productrestockalert
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductRestockAlert alert)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _alertRepository.AddAsync(alert);
            return CreatedAtAction(nameof(GetById), new { id = alert.Id }, alert);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating alert: {ex.Message}");
        }
    }

    // PUT: api/productrestockalert/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductRestockAlert alert)
    {
        try
        {
            if (id != alert.Id)
                return BadRequest("Mismatched alert ID in request.");

            var existing = await _alertRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Alert with ID {id} not found.");

            await _alertRepository.UpdateAsync(alert);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating alert: {ex.Message}");
        }
    }

    // DELETE: api/productrestockalert/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var alert = await _alertRepository.GetByIdAsync(id);
            if (alert == null)
                return NotFound($"Alert with ID {id} not found.");

            await _alertRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting alert: {ex.Message}");
        }
    }
}
