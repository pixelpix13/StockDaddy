using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

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
    public async Task<ActionResult<List<AuditLog>>> GetAll()
    {
        var logs = await _auditLogRepository.GetAllAsync();
        return Ok(logs);
    }

    // GET: api/AuditLog/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<AuditLog>> GetById(Guid id)
    {
        var log = await _auditLogRepository.GetByIdAsync(id);
        if (log == null)
            return NotFound($"Audit log with ID {id} not found.");

        return Ok(log);
    }

    // POST: api/AuditLog
    [HttpPost]
    public async Task<ActionResult> Add(AuditLog auditLog)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _auditLogRepository.AddAsync(auditLog);
        return CreatedAtAction(nameof(GetById), new { id = auditLog.Id }, auditLog);
    }
}
    