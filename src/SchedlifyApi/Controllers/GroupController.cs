using System.Runtime.CompilerServices;

namespace SchedlifyApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

using Models;
using Repositories;
using Attributes;
using DTO;


[ApiController]
[Route("/groups")]
public class GroupsController : ControllerBase
{
    private readonly IGroupRepository _repository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUserRepository _userRepository;

    public GroupsController(
        IGroupRepository repository,
        IDepartmentRepository departmentRepository,
        IUserRepository userRepository)
    {
        _repository = repository;
        _departmentRepository = departmentRepository;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<GroupResponse>>> GetGroups(
        [FromQuery] int? departmentId = null,
        [FromQuery] int? administratorId = null,
        [FromQuery] string? s = null,
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 10
    )
    {
        // offset and limit does not work
        var departments = await _repository.GetAll(
            departmentId, administratorId, s, offset, limit);
        var response = departments.Select(d => new GroupResponse
        {
            Id = d.Id,
            Name = d.Name,
            DepartmentId = d.DepartmentId
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{groupId:int}")]
    public async Task<ActionResult<GroupExtendedResponse>> GetGroupById(int groupId)
    { // offset and limit does not work
        var group = await _repository.GetById(groupId);
        if (group == null)
        {
            return BadRequest("Group does not exist.");
        }

        var response = new GroupExtendedResponse()
        {
            Name = group.Name,
            DepartmentId = group.DepartmentId,
            Department = new DepartmentResponse
            {
                Id = group.DepartmentId,
                Name = group.Department.Name
            },
            University = new UniversityResponse
            {
                Id = group.Department.UniversityId,
                Name = group.Department.University.Name
            }
        };
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<GroupResponse>> CreateGroup(
        [FromBody] CreateGroupRequest request,
        [FromHeader(Name = "X-CLIENT-UID")] int userId
    )
    {
        var department = await _departmentRepository.GetById(request.DepartmentId);
        if (department == null)
        {
            return BadRequest("Department does not exist.");
        }
        var administrator = await _userRepository.GetUserById(userId);
        if (administrator == null)
        {
            return BadRequest("User does not exist.");
        }

        var group = new Group
        {
            Name = request.Name,
            DepartmentId = request.DepartmentId,
            AdministratorId = userId
        };

        var createdGroup = await _repository.Create(group);
        var response = new GroupResponse
        {
            Id = createdGroup.Id,
            Name = createdGroup.Name,
            DepartmentId = createdGroup.DepartmentId
        };

        return CreatedAtAction(nameof(GetGroups), response);
    }
}