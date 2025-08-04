using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RefundController : ControllerBase
{
    private readonly IRefundRepository _refundRepository;

    public RefundController(IRefundRepository refundRepository)
    {
        _refundRepository = refundRepository;
    }

    // GET: api/refund
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var refunds = await _refundRepository.GetAllAsync();
            return Ok(refunds);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching refunds: {ex.Message}");
        }
    }

    // GET: api/refund/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var refund = await _refundRepository.GetByIdAsync(id);
            if (refund == null)
                return NotFound($"Refund with ID {id} not found.");

            return Ok(refund);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching the refund: {ex.Message}");
        }
    }

    // POST: api/refund
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Refund refund)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _refundRepository.AddAsync(refund);
            return CreatedAtAction(nameof(GetById), new { id = refund.Id }, refund);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the refund: {ex.Message}");
        }
    }

    // PUT: api/refund/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Refund refund)
    {
        try
        {
            if (id != refund.Id)
                return BadRequest("ID in URL does not match the refund object.");

            var existing = await _refundRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Refund with ID {id} not found.");

            await _refundRepository.UpdateAsync(refund);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the refund: {ex.Message}");
        }
    }

    // DELETE: api/refund/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var existing = await _refundRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Refund with ID {id} not found.");

            await _refundRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the refund: {ex.Message}");
        }
    }
}
