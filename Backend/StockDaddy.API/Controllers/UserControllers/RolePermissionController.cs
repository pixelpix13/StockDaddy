using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolePermissionController : ControllerBase
{
    private readonly IRolePermissionRepository _rolePermissionRepository;

    public RolePermissionController(IRolePermissionRepository rolePermissionRepository)
    {
        _rolePermissionRepository = rolePermissionRepository;
    }

    // GET: api/rolepermission
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var permissions = await _rolePermissionRepository.GetAllAsync();
            return Ok(permissions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching role-permissions: {ex.Message}");
        }
    }

    // GET: api/rolepermission/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var rolePermission = await _rolePermissionRepository.GetByIdAsync(id);
            if (rolePermission == null)
                return NotFound($"RolePermission with ID {id} not found.");

            return Ok(rolePermission);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching the role-permission: {ex.Message}");
        }
    }

    // POST: api/rolepermission
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RolePermission rolePermission)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _rolePermissionRepository.AddAsync(rolePermission);
            return CreatedAtAction(nameof(GetById), new { id = rolePermission.Id }, rolePermission);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the role-permission: {ex.Message}");
        }
    }

    // PUT: api/rolepermission/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] RolePermission rolePermission)
    {
        try
        {
            if (id != rolePermission.Id)
                return BadRequest("ID in URL does not match the role-permission object.");

            var existing = await _rolePermissionRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"RolePermission with ID {id} not found.");

            await _rolePermissionRepository.UpdateAsync(rolePermission);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the role-permission: {ex.Message}");
        }
    }

    // DELETE: api/rolepermission/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var existing = await _rolePermissionRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"RolePermission with ID {id} not found.");

            await _rolePermissionRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the role-permission: {ex.Message}");
        }
    }
}
