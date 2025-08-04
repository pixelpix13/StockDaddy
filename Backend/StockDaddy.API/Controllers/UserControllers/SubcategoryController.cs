using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubcategoryController : ControllerBase
{
    private readonly ISubcategoryRepository _subcategoryRepository;

    public SubcategoryController(ISubcategoryRepository subcategoryRepository)
    {
        _subcategoryRepository = subcategoryRepository;
    }

    // GET: api/subcategory
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var subcategories = await _subcategoryRepository.GetAllAsync();
            return Ok(subcategories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching subcategories: {ex.Message}");
        }
    }

    // GET: api/subcategory/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var subcategory = await _subcategoryRepository.GetByIdAsync(id);
            if (subcategory == null)
                return NotFound($"Subcategory with ID {id} not found.");

            return Ok(subcategory);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching subcategory: {ex.Message}");
        }
    }

    // POST: api/subcategory
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Subcategory subcategory)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _subcategoryRepository.AddAsync(subcategory);
            return CreatedAtAction(nameof(GetById), new { id = subcategory.Id }, subcategory);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating subcategory: {ex.Message}");
        }
    }

    // PUT: api/subcategory/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Subcategory subcategory)
    {
        try
        {
            if (id != subcategory.Id)
                return BadRequest("ID in URL does not match the subcategory object.");

            var existing = await _subcategoryRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Subcategory with ID {id} not found.");

            await _subcategoryRepository.UpdateAsync(subcategory);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating subcategory: {ex.Message}");
        }
    }

    // DELETE: api/subcategory/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var existing = await _subcategoryRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Subcategory with ID {id} not found.");

            await _subcategoryRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting subcategory: {ex.Message}");
        }
    }
}
