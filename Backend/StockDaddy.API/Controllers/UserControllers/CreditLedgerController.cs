using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CreditLedgerController : ControllerBase
{
    private readonly ICreditLedgerRepository _creditLedgerRepository;

    public CreditLedgerController(ICreditLedgerRepository creditLedgerRepository)
    {
        _creditLedgerRepository = creditLedgerRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedQuery query)
    {
        var result = await _creditLedgerRepository.GetPagedAsync(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entry = await _creditLedgerRepository.GetByIdAsync(id);
        if (entry == null) return NotFound($"Credit entry #{id} not found.");
        return Ok(entry);
    }

    [HttpPost("{id}/payments")]
    public async Task<IActionResult> RecordPayment(int id, [FromBody] RecordCreditPaymentRequest request)
    {
        try
        {
            var result = await _creditLedgerRepository.RecordPaymentAsync(id, request);
            if (result == null) return NotFound($"Credit entry #{id} not found.");
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCreditLedgerRequest request)
    {
        var result = await _creditLedgerRepository.UpdateAsync(id, request);
        if (result == null) return NotFound($"Credit entry #{id} not found.");
        return Ok(result);
    }
}
