using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceController(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    // GET: api/invoice
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var invoices = await _invoiceRepository.GetAllAsync();
            return Ok(invoices);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to retrieve invoices: {ex.Message}");
        }
    }

    // GET: api/invoice/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
                return NotFound($"Invoice with ID {id} not found.");

            return Ok(invoice);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to fetch invoice: {ex.Message}");
        }
    }

    // POST: api/invoice
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Invoice invoice)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _invoiceRepository.AddAsync(invoice);
            return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to create invoice: {ex.Message}");
        }
    }

    // PUT: api/invoice/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Invoice invoice)
    {
        try
        {
            if (id != invoice.Id)
                return BadRequest("Invoice ID mismatch between URL and body.");

            var existing = await _invoiceRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Invoice with ID {id} not found.");

            await _invoiceRepository.UpdateAsync(invoice);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to update invoice: {ex.Message}");
        }
    }

    // DELETE: api/invoice/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
                return NotFound($"Invoice with ID {id} not found.");

            await _invoiceRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to delete invoice: {ex.Message}");
        }
    }
}
