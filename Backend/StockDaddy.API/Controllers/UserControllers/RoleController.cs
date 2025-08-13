using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleRepository _roleRepository;

    public RoleController(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    // GET: api/role
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var roles = await _roleRepository.GetAllAsync();
            return Ok(roles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching roles: {ex.Message}");
        }
    }

    // GET: api/role/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                return NotFound($"Role with ID {id} not found.");
            return Ok(role);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching the role: {ex.Message}");
        }
    }

    // POST: api/role
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _roleRepository.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the role: {ex.Message}");
        }
    }

    // PUT: api/role/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleRequest request)
    {
        try
        {
            var existing = await _roleRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Role with ID {id} not found.");

            await _roleRepository.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the role: {ex.Message}");
        }
    }

    // DELETE: api/role/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var existing = await _roleRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Role with ID {id} not found.");

            await _roleRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the role: {ex.Message}");
        }
    }
}
