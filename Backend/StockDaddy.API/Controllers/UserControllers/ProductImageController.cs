using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductImageController : ControllerBase
{
    private readonly IProductImageRepository _productImageRepository;

    public ProductImageController(IProductImageRepository productImageRepository)
    {
        _productImageRepository = productImageRepository;
    }

    // GET: api/productimage
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var images = await _productImageRepository.GetAllAsync();
            return Ok(images);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to retrieve product images: {ex.Message}");
        }
    }

    // GET: api/productimage/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var image = await _productImageRepository.GetByIdAsync(id);
            if (image == null)
                return NotFound($"Product image with ID {id} not found.");
            return Ok(image);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to retrieve product image: {ex.Message}");
        }
    }

    // POST: api/productimage
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductImageRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _productImageRepository.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to create product image: {ex.Message}");
        }
    }

    // PUT: api/productimage/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductImageRequest request)
    {
        try
        {
            var existing = await _productImageRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Product image with ID {id} not found.");

            await _productImageRepository.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to update product image: {ex.Message}");
        }
    }

    // DELETE: api/productimage/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        try
        {
            var image = await _productImageRepository.GetByIdAsync(id);
            if (image == null)
                return NotFound($"Product image with ID {id} not found.");

            await _productImageRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to delete product image: {ex.Message}");
        }
    }
}
