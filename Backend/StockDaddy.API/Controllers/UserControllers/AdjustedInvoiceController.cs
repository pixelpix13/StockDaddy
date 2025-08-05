using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

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
    public async Task<ActionResult<List<AdjustedInvoiceDto>>> GetAll()
    {
        var invoices = await _adjustedInvoiceRepository.GetAllAsync();
        return Ok(invoices);
    }

    // GET: api/AdjustedInvoice/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<AdjustedInvoiceDto>> GetById(int id)
    {
        var invoice = await _adjustedInvoiceRepository.GetByIdAsync(id);
        if (invoice == null)
            return NotFound($"Adjusted invoice with ID {id} not found.");

        return Ok(invoice);
    }

    // POST: api/AdjustedInvoice
    [HttpPost]
    public async Task<ActionResult> Add([FromBody] CreateAdjustedInvoiceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _adjustedInvoiceRepository.AddAsync(request);
        // Optionally, fetch the created entity's ID if needed
        return Ok();
    }

    // PUT: api/AdjustedInvoice/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateAdjustedInvoiceRequest request)
    {
        var existing = await _adjustedInvoiceRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound($"Adjusted invoice with ID {id} not found.");

        await _adjustedInvoiceRepository.UpdateAsync(id, request);
        return NoContent();
    }

    // DELETE: api/AdjustedInvoice/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existing = await _adjustedInvoiceRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound($"Adjusted invoice with ID {id} not found.");

        await _adjustedInvoiceRepository.DeleteAsync(id);
        return NoContent();
    }
}
