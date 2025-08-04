using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BundleSaleItemController : ControllerBase
{
    private readonly IBundleSaleItemRepository _repo;

    public BundleSaleItemController(IBundleSaleItemRepository repo)
    {
        _repo = repo;
    }

    // GET: api/bundlesaleitem
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var items = await _repo.GetAllAsync();
            return Ok(items);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching items: {ex.Message}");
        }
    }

    // GET: api/bundlesaleitem/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null)
                return NotFound($"Item with ID {id} not found.");

            return Ok(item);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving item: {ex.Message}");
        }
    }

    // POST: api/bundlesaleitem
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BundleSaleItem item)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _repo.AddAsync(item);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating item: {ex.Message}");
        }
    }

    // PUT: api/bundlesaleitem/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] BundleSaleItem item)
    {
        try
        {
            if (id != item.Id)
                return BadRequest("ID in URL and body do not match.");

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Item with ID {id} not found.");

            await _repo.UpdateAsync(item);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating item: {ex.Message}");
        }
    }

    // DELETE: api/bundlesaleitem/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDelete(Guid id)
    {
        try
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null)
                return NotFound($"Item with ID {id} not found.");

            await _repo.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting item: {ex.Message}");
        }
    }
}
