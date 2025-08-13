using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionController : ControllerBase
{
    private readonly IPermissionRepository _permissionRepository;

    public PermissionController(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    // GET: api/permission
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var permissions = await _permissionRepository.GetAllAsync();
            return Ok(permissions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to retrieve permissions: {ex.Message}");
        }
    }

    // GET: api/permission/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var permission = await _permissionRepository.GetByIdAsync(id);
            if (permission == null)
                return NotFound($"Permission with ID {id} not found.");
            return Ok(permission);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to fetch permission: {ex.Message}");
        }
    }

    // POST: api/permission
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePermissionRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _permissionRepository.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to create permission: {ex.Message}");
        }
    }

    // PUT: api/permission/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePermissionRequest request)
    {
        try
        {
            var existing = await _permissionRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Permission with ID {id} not found.");

            await _permissionRepository.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to update permission: {ex.Message}");
        }
    }

    // DELETE: api/permission/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var permission = await _permissionRepository.GetByIdAsync(id);
            if (permission == null)
                return NotFound($"Permission with ID {id} not found.");

            await _permissionRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to delete permission: {ex.Message}");
        }
    }
}
