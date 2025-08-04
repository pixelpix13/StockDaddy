using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdjustedInvoiceController : ControllerBase
{
    private readonly IAdjustedInvoiceRepository _adjustedInvoiceRepository;

    public AdjustedInvoiceController(IAdjustedInvoiceRepository adjustedInvoiceRepository)
    {
        _adjustedInvoiceRepository = adjustedInvoiceRepository;
    }

    // GET: api/AdjustedInvoice
    [HttpGet]
    public async Task<ActionResult<List<AdjustedInvoice>>> GetAll()
    {
        var invoices = await _adjustedInvoiceRepository.GetAllAsync();
        return Ok(invoices);
    }

    // GET: api/AdjustedInvoice/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<AdjustedInvoice>> GetById(Guid id)
    {
        var invoice = await _adjustedInvoiceRepository.GetByIdAsync(id);
        if (invoice == null)
            return NotFound($"Adjusted invoice with ID {id} not found.");

        return Ok(invoice);
    }

    // POST: api/AdjustedInvoice
    [HttpPost]
    public async Task<ActionResult> Add(AdjustedInvoice invoice)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _adjustedInvoiceRepository.AddAsync(invoice);
        return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
    }

    // PUT: api/AdjustedInvoice/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, AdjustedInvoice invoice)
    {
        if (id != invoice.Id)
            return BadRequest("Mismatched invoice ID.");

        await _adjustedInvoiceRepository.UpdateAsync(invoice);
        return NoContent();
    }

    // DELETE: api/AdjustedInvoice/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var existing = await _adjustedInvoiceRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound($"Adjusted invoice with ID {id} not found.");

        await _adjustedInvoiceRepository.DeleteAsync(id);
        return NoContent();
    }
}
