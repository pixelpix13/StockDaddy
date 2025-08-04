using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseOrderController : ControllerBase
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;

    public PurchaseOrderController(IPurchaseOrderRepository purchaseOrderRepository)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
    }

    // GET: api/purchaseorder
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var orders = await _purchaseOrderRepository.GetAllAsync();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching purchase orders: {ex.Message}");
        }
    }

    // GET: api/purchaseorder/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var order = await _purchaseOrderRepository.GetByIdAsync(id);
            if (order == null)
                return NotFound($"Purchase order with ID {id} not found.");

            return Ok(order);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching purchase order: {ex.Message}");
        }
    }

    // POST: api/purchaseorder
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PurchaseOrder order)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _purchaseOrderRepository.AddAsync(order);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating purchase order: {ex.Message}");
        }
    }

    // PUT: api/purchaseorder/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PurchaseOrder order)
    {
        try
        {
            if (id != order.Id)
                return BadRequest("ID in URL does not match the purchase order.");

            var existing = await _purchaseOrderRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Purchase order with ID {id} not found.");

            await _purchaseOrderRepository.UpdateAsync(order);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating purchase order: {ex.Message}");
        }
    }

    // DELETE: api/purchaseorder/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var existing = await _purchaseOrderRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Purchase order with ID {id} not found.");

            await _purchaseOrderRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting purchase order: {ex.Message}");
        }
    }
}
