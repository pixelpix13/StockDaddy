using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogController : ControllerBase
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogController(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    // GET: api/AuditLog
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedQuery query)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        ApplyActivityScope(query, currentUserId);

        var logs = await _auditLogRepository.GetPagedAsync(query);
        return Ok(logs);
    }

    // GET: api/AuditLog/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<AuditLogDto>> GetById(int id)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var log = await _auditLogRepository.GetByIdAsync(id);
        if (log == null)
        {
            return NotFound($"Audit log with ID {id} not found.");
        }

        if (!CanViewAllActivity() && log.UserId != currentUserId)
        {
            return Forbid();
        }

        return Ok(log);
    }

    // POST: api/AuditLog
    [HttpPost]
    public async Task<ActionResult> Add([FromBody] CreateAuditLogRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _auditLogRepository.AddAsync(request);
        return Ok();
    }

    private bool TryGetCurrentUserId(out int userId)
    {
        userId = 0;
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out userId);
    }

    private bool CanViewAllActivity() =>
        User.IsInRole("Admin") ||
        User.Claims.Any(c =>
            c.Type == ClaimTypes.Role &&
            string.Equals(c.Value, "Admin", StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Admins see all users' activity (optional userId filter). Others see only their own.
    /// </summary>
    private void ApplyActivityScope(PagedQuery query, int currentUserId)
    {
        if (CanViewAllActivity())
        {
            return;
        }

        query.UserId = currentUserId;
    }
}
