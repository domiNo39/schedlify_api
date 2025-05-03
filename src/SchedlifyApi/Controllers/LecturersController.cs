using System.Runtime.InteropServices.JavaScript;

namespace SchedlifyApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using Repositories;
using DTO;

[ApiController]
[Route("lecturers")]
public class LecturersController : ControllerBase
{
    private readonly IAssignmentRepository _assignmentRepository;

    public LecturersController(
        IAssignmentRepository assignmentRepository
    )
    {
        _assignmentRepository = assignmentRepository;
    }

    [HttpGet]
    async public Task<ActionResult<List<Lecturer>>> GetLecturers(
        int? universityId = null, int? departmentId = null)
    {
        List<Assignment> assignments = await _assignmentRepository.GetByFilters(universityId, departmentId);

        var lecturers = assignments
            .Where(a => !string.IsNullOrWhiteSpace(a.Lecturer))
            .Select(a => a.Lecturer!.Trim())
            .Distinct()
            .Select(name => new Lecturer { Name = name })
            .ToList();

        return Ok(lecturers); 
    }
}