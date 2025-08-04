using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductTagController : ControllerBase
{
    private readonly IProductTagRepository _productTagRepository;

    public ProductTagController(IProductTagRepository productTagRepository)
    {
        _productTagRepository = productTagRepository;
    }

    // GET: api/producttag
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var tags = await _productTagRepository.GetAllAsync();
            return Ok(tags);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to retrieve tags: {ex.Message}");
        }
    }

    // GET: api/producttag/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var tag = await _productTagRepository.GetByIdAsync(id);
            if (tag == null)
                return NotFound($"Tag with ID {id} not found.");

            return Ok(tag);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to retrieve tag: {ex.Message}");
        }
    }

    // POST: api/producttag
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductTag tag)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _productTagRepository.AddAsync(tag);
            return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tag);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to create tag: {ex.Message}");
        }
    }

    // PUT: api/producttag/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductTag tag)
    {
        try
        {
            if (id != tag.Id)
                return BadRequest("ID in URL does not match ID in body.");

            var existing = await _productTagRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Tag with ID {id} not found.");

            await _productTagRepository.UpdateAsync(tag);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to update tag: {ex.Message}");
        }
    }

    // DELETE: api/producttag/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var tag = await _productTagRepository.GetByIdAsync(id);
            if (tag == null)
                return NotFound($"Tag with ID {id} not found.");

            await _productTagRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to delete tag: {ex.Message}");
        }
    }
}
