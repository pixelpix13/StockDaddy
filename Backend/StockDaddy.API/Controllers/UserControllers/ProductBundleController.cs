using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductBundleController : ControllerBase
{
    private readonly IProductBundleRepository _productBundleRepository;

    public ProductBundleController(IProductBundleRepository productBundleRepository)
    {
        _productBundleRepository = productBundleRepository;
    }

    // GET: api/productbundle
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var bundles = await _productBundleRepository.GetAllAsync();
            return Ok(bundles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to retrieve product bundles: {ex.Message}");
        }
    }

    // GET: api/productbundle/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var bundle = await _productBundleRepository.GetByIdAsync(id);
            if (bundle == null)
                return NotFound($"Product bundle with ID {id} not found.");
            return Ok(bundle);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to retrieve product bundle: {ex.Message}");
        }
    }

    // POST: api/productbundle
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductBundleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _productBundleRepository.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to create product bundle: {ex.Message}");
        }
    }

    // PUT: api/productbundle/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductBundleRequest request)
    {
        try
        {
            var existing = await _productBundleRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Product bundle with ID {id} not found.");

            await _productBundleRepository.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to update product bundle: {ex.Message}");
        }
    }

    // DELETE: api/productbundle/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        try
        {
            var bundle = await _productBundleRepository.GetByIdAsync(id);
            if (bundle == null)
                return NotFound($"Product bundle with ID {id} not found.");

            await _productBundleRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to delete product bundle: {ex.Message}");
        }
    }
}
