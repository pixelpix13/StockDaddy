
using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Services;
using StockDaddy.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StockDaddy.API.Controllers.UserControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduledPriceRevertController : ControllerBase
    {
        private readonly ScheduledPriceRevertService _service;

        public ScheduledPriceRevertController(ScheduledPriceRevertService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<ScheduledPriceRevert>>> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<ScheduledPriceRevert?>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ScheduledPriceRevert>> Create([FromBody] CreateScheduledPriceRevertRequest request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ScheduledPriceRevert?>> Update(int id, [FromBody] UpdateScheduledPriceRevertRequest request)
        {
            var updated = await _service.UpdateAsync(id, request);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
