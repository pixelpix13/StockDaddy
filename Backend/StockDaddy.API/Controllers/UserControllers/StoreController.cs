using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoreController : ControllerBase
{
    private readonly IStoreRepository _storeRepository;

    public StoreController(IStoreRepository storeRepository)
    {
        _storeRepository = storeRepository;
    }

    // GET: api/store
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var stores = await _storeRepository.GetAllAsync();
            return Ok(stores);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching stores: {ex.Message}");
        }
    }

    // GET: api/store/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var store = await _storeRepository.GetByIdAsync(id);
            if (store == null)
                return NotFound($"Store with ID {id} not found.");
            return Ok(store);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching the store: {ex.Message}");
        }
    }

    // POST: api/store
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStoreRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _storeRepository.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the store: {ex.Message}");
        }
    }

    // PUT: api/store/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStoreRequest request)
    {
        try
        {
            var existing = await _storeRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Store with ID {id} not found.");

            await _storeRepository.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the store: {ex.Message}");
        }
    }

    // DELETE: api/store/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var existing = await _storeRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Store with ID {id} not found.");

            await _storeRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the store: {ex.Message}");
        }
    }
}
