using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BundleItemController : ControllerBase
{
    private readonly IBundleItemRepository _repository;

    public BundleItemController(IBundleItemRepository repository)
    {
        _repository = repository;
    }

    // GET: api/bundleitem
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var items = await _repository.GetAllAsync();
            return Ok(items);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching bundle items: {ex.Message}");
        }
    }

    // GET: api/bundleitem/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
                return NotFound($"Bundle item with ID {id} not found.");

            return Ok(item);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching item: {ex.Message}");
        }
    }

    // POST: api/bundleitem
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BundleItem item)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _repository.AddAsync(item);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating bundle item: {ex.Message}");
        }
    }

    // PUT: api/bundleitem/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] BundleItem item)
    {
        try
        {
            if (id != item.Id)
                return BadRequest("ID mismatch between URL and body.");

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Bundle item with ID {id} not found.");

            await _repository.UpdateAsync(item);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating bundle item: {ex.Message}");
        }
    }

    // DELETE: api/bundleitem/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDelete(Guid id)
    {
        try
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
                return NotFound($"Bundle item with ID {id} not found.");

            await _repository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting bundle item: {ex.Message}");
        }
    }
}
