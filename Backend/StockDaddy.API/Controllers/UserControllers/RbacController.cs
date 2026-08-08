using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Authorization;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Services;

namespace StockDaddy.API.Controllers;

/// <summary>
/// In-app RBAC management: permission matrix and user role assignment.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RbacController : ControllerBase
{
    private readonly RbacService _rbacService;

    public RbacController(RbacService rbacService)
    {
        _rbacService = rbacService;
    }

    [HttpGet("matrix")]
    public async Task<IActionResult> GetMatrix()
    {
        var matrix = await _rbacService.GetMatrixAsync();
        return Ok(matrix);
    }

    [HttpGet("permissions")]
    public async Task<IActionResult> GetPermissions()
    {
        var permissions = await _rbacService.GetPermissionsAsync();
        return Ok(permissions);
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _rbacService.GetRolesWithPermissionsAsync();
        return Ok(roles);
    }

    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        var (result, error) = await _rbacService.CreateRoleAsync(request);
        if (result == null)
        {
            return BadRequest(new { message = error });
        }

        return Ok(result);
    }

    [HttpPut("roles/{roleId}")]
    public async Task<IActionResult> UpdateRole(int roleId, [FromBody] UpdateRoleRequest request)
    {
        var (result, error) = await _rbacService.UpdateRoleAsync(roleId, request);
        if (result == null)
        {
            return error == "Role not found." ? NotFound(new { message = error }) : BadRequest(new { message = error });
        }

        return Ok(result);
    }

    [HttpDelete("roles/{roleId}")]
    public async Task<IActionResult> DeleteRole(int roleId)
    {
        var (success, error) = await _rbacService.DeleteRoleAsync(roleId);
        if (!success)
        {
            return error == "Role not found." ? NotFound(new { message = error }) : BadRequest(new { message = error });
        }

        return NoContent();
    }

    [HttpPut("roles/{roleId}/permissions")]
    public async Task<IActionResult> UpdateRolePermissions(int roleId, [FromBody] UpdateRolePermissionsRequest request)
    {
        var result = await _rbacService.UpdateRolePermissionsAsync(roleId, request);
        if (result == null)
        {
            return NotFound($"Role {roleId} not found.");
        }

        return Ok(result);
    }

    [HttpPut("users/{userId}/role")]
    public async Task<IActionResult> AssignUserRole(int userId, [FromBody] AssignUserRoleRequest request)
    {
        var result = await _rbacService.AssignUserRoleAsync(userId, request);
        if (result == null)
        {
            return NotFound($"User {userId} or role {request.RoleId} not found.");
        }

        return Ok(result);
    }

    [HttpPut("users/{userId}/store-assignments")]
    public async Task<IActionResult> AssignUserStoreAssignments(int userId, [FromBody] AssignUserStoreAssignmentsRequest request)
    {
        var result = await _rbacService.AssignUserStoreAssignmentsAsync(userId, request);
        if (result == null)
        {
            return NotFound($"User {userId} not found or invalid store/role assignment.");
        }

        return Ok(result);
    }
}
