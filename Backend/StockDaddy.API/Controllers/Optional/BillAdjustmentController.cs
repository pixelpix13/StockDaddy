using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StockDaddy.API.Configuration;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;

namespace StockDaddy.API.Controllers;

/// <summary>
/// Optional, removable module for post-sale bill corrections.
/// Disable via Features:BillAdjustment:Enabled=false and remove this controller folder.
/// </summary>
[ApiController]
[Route("api/bill-adjustment")]
public class BillAdjustmentController : ControllerBase
{
    private readonly ISaleRepository _saleRepository;
    private readonly FeatureOptions _features;

    public BillAdjustmentController(ISaleRepository saleRepository, IOptions<FeatureOptions> features)
    {
        _saleRepository = saleRepository;
        _features = features.Value;
    }

    [HttpPut("{saleId}")]
    public async Task<IActionResult> AdjustSale(int saleId, [FromBody] UpdateSaleRequest request)
    {
        if (!_features.BillAdjustment.Enabled)
            return NotFound(new { message = "Bill adjustment feature is disabled." });

        try
        {
            var existing = await _saleRepository.GetByIdAsync(saleId);
            if (existing == null)
                return NotFound($"Sale with ID {saleId} not found.");

            await _saleRepository.UpdateAsync(saleId, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while adjusting the sale: {ex.Message}");
        }
    }

    [HttpDelete("{saleId}")]
    public async Task<IActionResult> VoidSale(int saleId)
    {
        if (!_features.BillAdjustment.Enabled)
            return NotFound(new { message = "Bill adjustment feature is disabled." });

        try
        {
            var existing = await _saleRepository.GetByIdAsync(saleId);
            if (existing == null)
                return NotFound($"Sale with ID {saleId} not found.");

            await _saleRepository.DeleteAsync(saleId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while voiding the sale: {ex.Message}");
        }
    }
}
