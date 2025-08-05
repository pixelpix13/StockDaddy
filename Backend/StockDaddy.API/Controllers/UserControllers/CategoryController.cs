using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepo;

    public CategoryController(ICategoryRepository categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }

    // GET: api/category
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var categories = await _categoryRepo.GetAllAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching categories: {ex.Message}");
        }
    }

    // GET: api/category/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
                return NotFound($"Category with ID {id} not found.");
            return Ok(category);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving category: {ex.Message}");
        }
    }

    // POST: api/category
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _categoryRepo.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating category: {ex.Message}");
        }
    }

    // PUT: api/category/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request)
    {
        try
        {
            var existing = await _categoryRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Category with ID {id} not found.");

            await _categoryRepo.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating category: {ex.Message}");
        }
    }

    // DELETE: api/category/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        try
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
                return NotFound($"Category with ID {id} not found.");

            await _categoryRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting category: {ex.Message}");
        }
    }
}
