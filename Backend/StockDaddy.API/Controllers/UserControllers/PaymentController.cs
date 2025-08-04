using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentController(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    // GET: api/payment
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var payments = await _paymentRepository.GetAllAsync();
            return Ok(payments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to retrieve payments: {ex.Message}");
        }
    }

    // GET: api/payment/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
                return NotFound($"Payment with ID {id} not found.");

            return Ok(payment);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to fetch payment: {ex.Message}");
        }
    }

    // POST: api/payment
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Payment payment)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _paymentRepository.AddAsync(payment);
            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to create payment: {ex.Message}");
        }
    }

    // PUT: api/payment/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Payment payment)
    {
        try
        {
            if (id != payment.Id)
                return BadRequest("ID in URL does not match ID in body.");

            var existing = await _paymentRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Payment with ID {id} not found.");

            await _paymentRepository.UpdateAsync(payment);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to update payment: {ex.Message}");
        }
    }

    // DELETE: api/payment/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
                return NotFound($"Payment with ID {id} not found.");

            await _paymentRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to delete payment: {ex.Message}");
        }
    }
}
