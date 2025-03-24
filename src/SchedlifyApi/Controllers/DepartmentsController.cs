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
    public async Task<ActionResult<List<DepartmentResponse>>> GetDepartments([FromQuery] int universityId, [FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        // offset and limit does not work
        var departments = await _repository.GetAll(universityId, offset, limit);
        var response = departments.Select(d => new DepartmentResponse
        {
            Id = d.Id,
            Name = d.Name,
            UniversityId = d.UniversityId
        }).ToList();

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
            UniversityId = createdDepartment.UniversityId
        };

        return CreatedAtAction(nameof(GetDepartments), response);
    }
}