using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GiftOptionController : ControllerBase
{
    private readonly IGiftOptionRepository _giftOptionRepo;

    public GiftOptionController(IGiftOptionRepository giftOptionRepo)
    {
        _giftOptionRepo = giftOptionRepo;
    }

    // GET: api/giftoption
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var options = await _giftOptionRepo.GetAllAsync();
            return Ok(options);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to retrieve gift options: {ex.Message}");
        }
    }

    // GET: api/giftoption/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var option = await _giftOptionRepo.GetByIdAsync(id);
            if (option == null)
                return NotFound($"Gift option with ID {id} not found.");
            return Ok(option);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving gift option: {ex.Message}");
        }
    }

    // POST: api/giftoption
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGiftOptionRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _giftOptionRepo.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to create gift option: {ex.Message}");
        }
    }

    // PUT: api/giftoption/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateGiftOptionRequest request)
    {
        try
        {
            var existing = await _giftOptionRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Gift option with ID {id} not found.");

            await _giftOptionRepo.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to update gift option: {ex.Message}");
        }
    }

    // DELETE: api/giftoption/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var existing = await _giftOptionRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Gift option with ID {id} not found.");

            await _giftOptionRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to delete gift option: {ex.Message}");
        }
    }
}
