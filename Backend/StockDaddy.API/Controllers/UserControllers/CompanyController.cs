using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyController(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedQuery query)
    {
        try
        {
            var companies = await _companyRepository.GetPagedAsync(query);
            return Ok(companies);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching companies: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null)
                return NotFound($"Company with ID {id} not found.");
            return Ok(company);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching the company: {ex.Message}");
        }
    }

    [HttpGet("{id}/sales")]
    public async Task<IActionResult> GetSalesHistory(int id, [FromQuery] PagedQuery query)
    {
        try
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null)
                return NotFound($"Company with ID {id} not found.");

            var history = await _companyRepository.GetSalesHistoryAsync(id, query);
            return Ok(history);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching company sales: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _companyRepository.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the company: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCompanyRequest request)
    {
        try
        {
            var existing = await _companyRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Company with ID {id} not found.");

            await _companyRepository.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the company: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var existing = await _companyRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Company with ID {id} not found.");

            await _companyRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the company: {ex.Message}");
        }
    }
}
