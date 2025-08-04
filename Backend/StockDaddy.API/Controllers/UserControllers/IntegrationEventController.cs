using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IntegrationEventController : ControllerBase
{
    private readonly IIntegrationEventRepository _eventRepo;

    public IntegrationEventController(IIntegrationEventRepository eventRepo)
    {
        _eventRepo = eventRepo;
    }

    // GET: api/integrationevent
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var events = await _eventRepo.GetAllAsync();
            return Ok(events);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving events: {ex.Message}");
        }
    }

    // GET: api/integrationevent/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var e = await _eventRepo.GetByIdAsync(id);
            if (e == null)
                return NotFound($"Event with ID {id} not found.");

            return Ok(e);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving event: {ex.Message}");
        }
    }

    // POST: api/integrationevent
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] IntegrationEvent e)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _eventRepo.AddAsync(e);
            return CreatedAtAction(nameof(GetById), new { id = e.Id }, e);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating event: {ex.Message}");
        }
    }

    // PUT: api/integrationevent/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] IntegrationEvent e)
    {
        try
        {
            if (id != e.Id)
                return BadRequest("Event ID mismatch between URL and payload.");

            var existing = await _eventRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Event with ID {id} not found.");

            await _eventRepo.UpdateAsync(e);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating event: {ex.Message}");
        }
    }

    // DELETE: api/integrationevent/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var existing = await _eventRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Event with ID {id} not found.");

            await _eventRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting event: {ex.Message}");
        }
    }
}
