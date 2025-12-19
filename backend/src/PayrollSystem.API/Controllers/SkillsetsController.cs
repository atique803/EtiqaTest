using Microsoft.AspNetCore.Mvc;
using PayrollSystem.Application.DTOs;
using PayrollSystem.Domain.Interfaces;

namespace PayrollSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SkillsetsController : ControllerBase
{
    private readonly ISkillsetRepository _skillsetRepository;

    public SkillsetsController(ISkillsetRepository skillsetRepository)
    {
        _skillsetRepository = skillsetRepository;
    }

    /// <summary>
    /// Get all skillsets
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SkillsetDto>>> GetAll()
    {
        try
        {
            var skillsets = await _skillsetRepository.GetAllAsync();
            var dtos = skillsets.Select(s => new SkillsetDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description
            });
            
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving skillsets", error = ex.Message });
        }
    }

    /// <summary>
    /// Get skillset by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SkillsetDto>> GetById(int id)
    {
        try
        {
            var skillset = await _skillsetRepository.GetByIdAsync(id);
            if (skillset == null)
                return NotFound(new { message = "Skillset not found" });

            var dto = new SkillsetDto
            {
                Id = skillset.Id,
                Name = skillset.Name,
                Description = skillset.Description
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving skillset", error = ex.Message });
        }
    }
}
