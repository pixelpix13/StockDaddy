using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaxRegionController : ControllerBase
{
    private readonly ITaxRegionRepository _taxRegionRepository;

    public TaxRegionController(ITaxRegionRepository taxRegionRepository)
    {
        _taxRegionRepository = taxRegionRepository;
    }

    // GET: api/taxregion
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var regions = await _taxRegionRepository.GetAllAsync();
            return Ok(regions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching tax regions: {ex.Message}");
        }
    }

    // GET: api/taxregion/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var region = await _taxRegionRepository.GetByIdAsync(id);
            if (region == null)
                return NotFound($"TaxRegion with ID {id} not found.");

            return Ok(region);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching the tax region: {ex.Message}");
        }
    }

    // POST: api/taxregion
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaxRegion region)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _taxRegionRepository.AddAsync(region);
            return CreatedAtAction(nameof(GetById), new { id = region.Id }, region);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the tax region: {ex.Message}");
        }
    }

    // PUT: api/taxregion/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TaxRegion region)
    {
        try
        {
            if (id != region.Id)
                return BadRequest("ID in URL does not match the request body.");

            var existing = await _taxRegionRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"TaxRegion with ID {id} not found.");

            await _taxRegionRepository.UpdateAsync(region);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the tax region: {ex.Message}");
        }
    }

    // DELETE: api/taxregion/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var region = await _taxRegionRepository.GetByIdAsync(id);
            if (region == null)
                return NotFound($"TaxRegion with ID {id} not found.");

            await _taxRegionRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the tax region: {ex.Message}");
        }
    }
}
