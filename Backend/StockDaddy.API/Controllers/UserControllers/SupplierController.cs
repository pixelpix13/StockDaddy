using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupplierController : ControllerBase
{
    private readonly ISupplierRepository _supplierRepository;

    public SupplierController(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    // GET: api/supplier
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            return Ok(suppliers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching suppliers: {ex.Message}");
        }
    }

    // GET: api/supplier/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                return NotFound($"Supplier with ID {id} not found.");
            return Ok(supplier);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching the supplier: {ex.Message}");
        }
    }

    // POST: api/supplier
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSupplierRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _supplierRepository.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the supplier: {ex.Message}");
        }
    }

    // PUT: api/supplier/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSupplierRequest request)
    {
        try
        {
            var existing = await _supplierRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Supplier with ID {id} not found.");

            await _supplierRepository.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the supplier: {ex.Message}");
        }
    }

    // DELETE: api/supplier/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var existing = await _supplierRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Supplier with ID {id} not found.");

            await _supplierRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the supplier: {ex.Message}");
        }
    }
}
