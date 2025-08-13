using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductAttributeController : ControllerBase
{
    private readonly IProductAttributeRepository _productAttributeRepository;

    public ProductAttributeController(IProductAttributeRepository productAttributeRepository)
    {
        _productAttributeRepository = productAttributeRepository;
    }

    // GET: api/productattribute
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var attributes = await _productAttributeRepository.GetAllAsync();
            return Ok(attributes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching product attributes: {ex.Message}");
        }
    }

    // GET: api/productattribute/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var attr = await _productAttributeRepository.GetByIdAsync(id);
            if (attr == null)
                return NotFound($"Product attribute with ID {id} not found.");
            return Ok(attr);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching product attribute: {ex.Message}");
        }
    }

    // POST: api/productattribute
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductAttributeRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _productAttributeRepository.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating product attribute: {ex.Message}");
        }
    }

    // PUT: api/productattribute/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductAttributeRequest request)
    {
        try
        {
            var existing = await _productAttributeRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Product attribute with ID {id} not found.");

            await _productAttributeRepository.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating product attribute: {ex.Message}");
        }
    }

    // DELETE: api/productattribute/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var existing = await _productAttributeRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Product attribute with ID {id} not found.");

            await _productAttributeRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting product attribute: {ex.Message}");
        }
    }
}
