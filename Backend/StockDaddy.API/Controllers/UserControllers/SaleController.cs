using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SaleController : ControllerBase
{
    private readonly ISaleRepository _saleRepository;

    public SaleController(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    // GET: api/sale
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var sales = await _saleRepository.GetAllAsync();
            return Ok(sales);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching sales: {ex.Message}");
        }
    }

    // GET: api/sale/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            if (sale == null)
                return NotFound($"Sale with ID {id} not found.");
            return Ok(sale);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching the sale: {ex.Message}");
        }
    }

    // POST: api/sale
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _saleRepository.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the sale: {ex.Message}");
        }
    }

    // PUT: api/sale/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSaleRequest request)
    {
        try
        {
            var existing = await _saleRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Sale with ID {id} not found.");

            await _saleRepository.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the sale: {ex.Message}");
        }
    }

    // DELETE: api/sale/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var existing = await _saleRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Sale with ID {id} not found.");

            await _saleRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the sale: {ex.Message}");
        }
    }
}
