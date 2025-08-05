using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipmentController : ControllerBase
{
    private readonly IShipmentRepository _shipmentRepository;

    public ShipmentController(IShipmentRepository shipmentRepository)
    {
        _shipmentRepository = shipmentRepository;
    }

    // GET: api/shipment
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var shipments = await _shipmentRepository.GetAllAsync();
            return Ok(shipments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching shipments: {ex.Message}");
        }
    }

    // GET: api/shipment/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var shipment = await _shipmentRepository.GetByIdAsync(id);
            if (shipment == null)
                return NotFound($"Shipment with ID {id} not found.");
            return Ok(shipment);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching shipment: {ex.Message}");
        }
    }

    // POST: api/shipment
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShipmentRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _shipmentRepository.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating shipment: {ex.Message}");
        }
    }

    // PUT: api/shipment/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateShipmentRequest request)
    {
        try
        {
            var existing = await _shipmentRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Shipment with ID {id} not found.");

            await _shipmentRepository.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating shipment: {ex.Message}");
        }
    }

    // DELETE: api/shipment/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var existing = await _shipmentRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Shipment with ID {id} not found.");

            await _shipmentRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting shipment: {ex.Message}");
        }
    }
}
