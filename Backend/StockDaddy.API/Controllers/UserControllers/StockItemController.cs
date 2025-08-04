using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockItemController : ControllerBase
{
    private readonly IStockItemRepository _stockItemRepository;

    public StockItemController(IStockItemRepository stockItemRepository)
    {
        _stockItemRepository = stockItemRepository;
    }

    // GET: api/stockitem
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var items = await _stockItemRepository.GetAllAsync();
            return Ok(items);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching stock items: {ex.Message}");
        }
    }

    // GET: api/stockitem/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var item = await _stockItemRepository.GetByIdAsync(id);
            if (item == null)
                return NotFound($"Stock item with ID {id} not found.");

            return Ok(item);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching the stock item: {ex.Message}");
        }
    }

    // POST: api/stockitem
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StockItem item)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _stockItemRepository.AddAsync(item);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the stock item: {ex.Message}");
        }
    }

    // PUT: api/stockitem/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] StockItem item)
    {
        try
        {
            if (id != item.Id)
                return BadRequest("ID in URL does not match the stock item object.");

            var existing = await _stockItemRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Stock item with ID {id} not found.");

            await _stockItemRepository.UpdateAsync(item);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the stock item: {ex.Message}");
        }
    }

    // DELETE: api/stockitem/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var existing = await _stockItemRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Stock item with ID {id} not found.");

            await _stockItemRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the stock item: {ex.Message}");
        }
    }
}
