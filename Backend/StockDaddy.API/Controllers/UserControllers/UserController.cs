using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // GET: api/user
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching users: {ex.Message}");
        }
    }

    // GET: api/user/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound($"User with ID {id} not found.");
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching user: {ex.Message}");
        }
    }

    // POST: api/user
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _userRepository.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating user: {ex.Message}");
        }
    }

    // PUT: api/user/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            var existing = await _userRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"User with ID {id} not found.");

            await _userRepository.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating user: {ex.Message}");
        }
    }

    // DELETE: api/user/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound($"User with ID {id} not found.");

            await _userRepository.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting user: {ex.Message}");
        }
    }
}
