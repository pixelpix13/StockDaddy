using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    // GET: api/product
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to fetch products: {ex.Message}");
        }
    }

    // GET: api/product/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound($"Product with ID {id} not found.");

            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to fetch product: {ex.Message}");
        }
    }

    // POST: api/product
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _productRepository.AddAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to create product: {ex.Message}");
        }
    }

    // PUT: api/product/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Product updated)
    {
        try
        {
            if (id != updated.Id)
                return BadRequest("ID in path and body do not match.");

            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Product with ID {id} not found.");

            await _productRepository.UpdateAsync(updated);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to update product: {ex.Message}");
        }
    }

    // DELETE: api/product/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDelete(Guid id)
    {
        try
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Product with ID {id} not found.");

            await _productRepository.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to delete product: {ex.Message}");
        }
    }
}
