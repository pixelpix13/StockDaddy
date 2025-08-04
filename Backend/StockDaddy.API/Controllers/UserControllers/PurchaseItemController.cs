using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseItemController : ControllerBase
{
    private readonly IPurchaseItemRepository _purchaseItemRepository;

    public PurchaseItemController(IPurchaseItemRepository purchaseItemRepository)
    {
        _purchaseItemRepository = purchaseItemRepository;
    }

    // GET: api/purchaseitem
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var items = await _purchaseItemRepository.GetAllAsync();
            return Ok(items);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching purchase items: {ex.Message}");
        }
    }

    // GET: api/purchaseitem/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var item = await _purchaseItemRepository.GetByIdAsync(id);
            if (item == null)
                return NotFound($"Purchase item with ID {id} not found.");

            return Ok(item);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching purchase item: {ex.Message}");
        }
    }

    // POST: api/purchaseitem
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PurchaseItem item)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _purchaseItemRepository.AddAsync(item);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating purchase item: {ex.Message}");
        }
    }

    // PUT: api/purchaseitem/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PurchaseItem item)
    {
        try
        {
            if (id != item.Id)
                return BadRequest("ID in URL does not match item ID.");

            var existing = await _purchaseItemRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Purchase item with ID {id} not found.");

            await _purchaseItemRepository.UpdateAsync(item);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating purchase item: {ex.Message}");
        }
    }

    // DELETE: api/purchaseitem/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var existing = await _purchaseItemRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Purchase item with ID {id} not found.");

            await _purchaseItemRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting purchase item: {ex.Message}");
        }
    }
}
