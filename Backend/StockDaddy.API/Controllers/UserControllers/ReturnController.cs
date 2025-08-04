using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReturnController : ControllerBase
{
    private readonly IReturnRepository _returnRepository;

    public ReturnController(IReturnRepository returnRepository)
    {
        _returnRepository = returnRepository;
    }

    // GET: api/return
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var returns = await _returnRepository.GetAllAsync();
            return Ok(returns);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching return records: {ex.Message}");
        }
    }

    // GET: api/return/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var r = await _returnRepository.GetByIdAsync(id);
            if (r == null)
                return NotFound($"Return with ID {id} not found.");

            return Ok(r);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching the return: {ex.Message}");
        }
    }

    // POST: api/return
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Return r)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _returnRepository.AddAsync(r);
            return CreatedAtAction(nameof(GetById), new { id = r.Id }, r);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the return: {ex.Message}");
        }
    }

    // PUT: api/return/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Return r)
    {
        try
        {
            if (id != r.Id)
                return BadRequest("ID in URL does not match the return object.");

            var existing = await _returnRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Return with ID {id} not found.");

            await _returnRepository.UpdateAsync(r);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the return: {ex.Message}");
        }
    }

    // DELETE: api/return/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var existing = await _returnRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Return with ID {id} not found.");

            await _returnRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the return: {ex.Message}");
        }
    }
}
