using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Services;

namespace StockDaddy.API.Controllers;

/// <summary>
/// HTTP surface for multi-step inventory/POS workflows. See OrchestrationService for logic.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrchestrationController : ControllerBase
{
    private readonly OrchestrationService _orchestrationService;

    public OrchestrationController(OrchestrationService orchestrationService)
    {
        _orchestrationService = orchestrationService;
    }

    [HttpPost("product-with-variant")]
    public async Task<IActionResult> CreateProductWithVariant([FromBody] CreateProductWithVariantRequest request)
    {
        try
        {
            var result = await _orchestrationService.CreateProductWithVariantAsync(request);
            if (result == null)
            {
                return BadRequest("Product name and SKU are required.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutSaleRequest request)
    {
        try
        {
            if (request.Items.Count == 0)
            {
                return BadRequest("At least one line item is required.");
            }

            var result = await _orchestrationService.CheckoutSaleAsync(request);
            if (result == null)
            {
                return BadRequest("One or more product variants were not found.");
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("adjust-stock")]
    public async Task<IActionResult> AdjustStock([FromBody] AdjustStockRequest request)
    {
        try
        {
            if (request.QuantityChange == 0)
            {
                return BadRequest("Quantity change must not be zero.");
            }

            var result = await _orchestrationService.AdjustStockAsync(request);
            if (result == null)
            {
                return NotFound("Product variant not found.");
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("variant-stock")]
    public async Task<IActionResult> GetVariantStock([FromQuery] int? storeId, [FromQuery] PagedQuery query)
    {
        try
        {
            var result = await _orchestrationService.GetVariantStockAsync(storeId, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("variant-by-barcode")]
    public async Task<IActionResult> GetVariantByBarcode([FromQuery] string code, [FromQuery] int storeId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("Barcode or SKU code is required.");
            }

            var result = await _orchestrationService.GetVariantByBarcodeAsync(code, storeId);
            if (result == null)
            {
                return NotFound($"No variant found for code '{code.Trim()}'.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("purchase-order-with-items")]
    public async Task<IActionResult> CreatePurchaseOrderWithItems(
        [FromBody] CreatePurchaseOrderWithItemsRequest request)
    {
        try
        {
            if (request.Items.Count == 0)
            {
                return BadRequest("At least one line item is required.");
            }

            var result = await _orchestrationService.CreatePurchaseOrderWithItemsAsync(request);
            if (result == null)
            {
                return BadRequest("Unable to create purchase order.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("purchase-order/{id}/receive")]
    public async Task<IActionResult> ReceivePurchaseOrder(int id)
    {
        try
        {
            var result = await _orchestrationService.ReceivePurchaseOrderAsync(id);
            if (result == null)
            {
                return NotFound($"Purchase order {id} not found.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
