namespace SchedlifyApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

using Models;
using Repositories;
using Attributes;
using DTO;


[ApiController]
[Route("/universities")]
public class UniversitiesController : ControllerBase
{
    private readonly IUniversityRepository _repository;

    public UniversitiesController(IUniversityRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<UniversityResponse>>> GetUniversities(
        [FromQuery] string? s = null,
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 10
    )
    {
        var universities = await _repository.GetAll(s, offset, limit);
        var response = universities.Select(u => new UniversityResponse
        {
            Id = u.Id,
            Name = u.Name
        }).ToList();

        return Ok(response);
    }
    

    [HttpGet("{id}")]
    public async Task<ActionResult<UniversityResponse>> GetUniversityById(
        int id
    )
    {
        var university = await _repository.GetById(id);
        if (university == null)
        {
            return NotFound("University not found");
        }
        var response = new UniversityResponse
        {
            Id = university.Id,
            Name = university.Name
        };
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<UniversityResponse>> CreateUniversity([FromBody] CreateUniversityRequest request)
    {
        var university = new University
        {
            Name = request.Name
        };

        var createdUniversity = await _repository.Create(university);
        var response = new UniversityResponse
        {
            Id = createdUniversity.Id,
            Name = createdUniversity.Name
        };

        return CreatedAtAction(nameof(GetUniversities), new { id = response.Id }, response);
    }
}
