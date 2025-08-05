using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductVariantController : ControllerBase
{
    private readonly IProductVariantRepository _productVariantRepository;

    public ProductVariantController(IProductVariantRepository productVariantRepository)
    {
        _productVariantRepository = productVariantRepository;
    }

    // GET: api/productvariant
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var variants = await _productVariantRepository.GetAllAsync();
            return Ok(variants);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving variants: {ex.Message}");
        }
    }

    // GET: api/productvariant/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var variant = await _productVariantRepository.GetByIdAsync(id);
            if (variant == null)
                return NotFound($"Product variant with ID {id} not found.");
            return Ok(variant);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving variant: {ex.Message}");
        }
    }

    // POST: api/productvariant
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductVariantRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _productVariantRepository.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating variant: {ex.Message}");
        }
    }

    // PUT: api/productvariant/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductVariantRequest request)
    {
        try
        {
            var existing = await _productVariantRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Product variant with ID {id} not found.");

            await _productVariantRepository.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating variant: {ex.Message}");
        }
    }

    // DELETE: api/productvariant/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var existing = await _productVariantRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Product variant with ID {id} not found.");

            await _productVariantRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting variant: {ex.Message}");
        }
    }
}
