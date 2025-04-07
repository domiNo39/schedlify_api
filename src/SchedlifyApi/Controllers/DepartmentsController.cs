namespace SchedlifyApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

using Models;
using Repositories;
using Attributes;
using DTO;


[ApiController]
[Route("/departments")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentRepository _repository;
    private readonly IUniversityRepository _universityRepository;

    public DepartmentsController(IDepartmentRepository repository, IUniversityRepository universityRepository)
    {
        _repository = repository;
        _universityRepository = universityRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<DepartmentResponse>>> GetDepartments(
        [FromQuery] int universityId,
        [FromQuery] string? s,
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 10
        )
    {
        // offset and limit does not work
        var departments = await _repository.GetAll(universityId, s, offset, limit);
        var response = departments.Select(d => new DepartmentResponse
        {
            Id = d.Id,
            Name = d.Name,
            UniversityId = d.UniversityId
        }).ToList();

        return Ok(response);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentResponse>> GetDepartmentById(
        int id
    )
    {
        var department = await _repository.GetById(id);
        if (department == null)
        {
            return NotFound("Department not found");
        }
        var response = new DepartmentResponse
        {
            Id = department.Id,
            Name = department.Name,
            UniversityId = department.UniversityId
        };
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentResponse>> CreateDepartment([FromBody] CreateDepartmentRequest request)
    {
        var university = await _universityRepository.GetById(request.UniversityId);
        if (university == null)
        {
            return BadRequest("University does not exist.");
        }

        var department = new Department
        {
            Name = request.Name,
            UniversityId = request.UniversityId
        };

        var createdDepartment = await _repository.Create(department);
        var response = new DepartmentResponse
        {
            Name = createdDepartment.Name,
            UniversityId = request.UniversityId //i hope this solution will work
        };

        return CreatedAtAction(nameof(GetDepartments), response);
    }
}