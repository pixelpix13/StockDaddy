using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

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
    public async Task<IActionResult> GetById(int id)
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
    public async Task<IActionResult> Create([FromBody] CreateTaxRegionRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _taxRegionRepository.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the tax region: {ex.Message}");
        }
    }

    // PUT: api/taxregion/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaxRegionRequest request)
    {
        try
        {
            var existing = await _taxRegionRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"TaxRegion with ID {id} not found.");

            await _taxRegionRepository.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the tax region: {ex.Message}");
        }
    }

    // DELETE: api/taxregion/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
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
