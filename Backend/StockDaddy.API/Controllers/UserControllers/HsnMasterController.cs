using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HsnMasterController : ControllerBase
{
    private readonly IHsnMasterRepository _hsnRepo;

    public HsnMasterController(IHsnMasterRepository hsnRepo)
    {
        _hsnRepo = hsnRepo;
    }

    // GET: api/hsnmaster
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var list = await _hsnRepo.GetAllAsync();
            return Ok(list);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to fetch HSNs: {ex.Message}");
        }
    }

    // GET: api/hsnmaster/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var hsn = await _hsnRepo.GetByIdAsync(id);
            if (hsn == null)
                return NotFound($"HSN with ID {id} not found.");
            return Ok(hsn);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching HSN: {ex.Message}");
        }
    }

    // POST: api/hsnmaster
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHsnMasterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _hsnRepo.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to create HSN: {ex.Message}");
        }
    }

    // PUT: api/hsnmaster/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHsnMasterRequest request)
    {
        try
        {
            var existing = await _hsnRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"HSN with ID {id} not found.");

            await _hsnRepo.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to update HSN: {ex.Message}");
        }
    }

    // DELETE: api/hsnmaster/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var existing = await _hsnRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"HSN with ID {id} not found.");

            await _hsnRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to delete HSN: {ex.Message}");
        }
    }
}
