using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SaleItemController : ControllerBase
{
    private readonly ISaleItemRepository _saleItemRepository;

    public SaleItemController(ISaleItemRepository saleItemRepository)
    {
        _saleItemRepository = saleItemRepository;
    }

    // GET: api/saleitem/by-sale/{saleId}
    [HttpGet("by-sale/{saleId}")]
    public async Task<IActionResult> GetBySaleId(int saleId)
    {
        try
        {
            var items = await _saleItemRepository.GetAllBySaleIdAsync(saleId);
            return Ok(items);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching sale items: {ex.Message}");
        }
    }

    // GET: api/saleitem/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var item = await _saleItemRepository.GetByIdAsync(id);
            if (item == null)
                return NotFound($"Sale item with ID {id} not found.");
            return Ok(item);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching sale item: {ex.Message}");
        }
    }

    // POST: api/saleitem
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleItemRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _saleItemRepository.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the sale item: {ex.Message}");
        }
    }

    // PUT: api/saleitem/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSaleItemRequest request)
    {
        try
        {
            var existing = await _saleItemRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Sale item with ID {id} not found.");

            await _saleItemRepository.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the sale item: {ex.Message}");
        }
    }

    // DELETE: api/saleitem/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var existing = await _saleItemRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Sale item with ID {id} not found.");

            await _saleItemRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the sale item: {ex.Message}");
        }
    }
}
