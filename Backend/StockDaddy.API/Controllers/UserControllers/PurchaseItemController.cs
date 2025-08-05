using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

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
    public async Task<IActionResult> GetById(int id)
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
    public async Task<IActionResult> Create([FromBody] CreatePurchaseItemRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _purchaseItemRepository.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating purchase item: {ex.Message}");
        }
    }

    // PUT: api/purchaseitem/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePurchaseItemRequest request)
    {
        try
        {
            var existing = await _purchaseItemRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Purchase item with ID {id} not found.");

            await _purchaseItemRepository.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating purchase item: {ex.Message}");
        }
    }

    // DELETE: api/purchaseitem/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
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
