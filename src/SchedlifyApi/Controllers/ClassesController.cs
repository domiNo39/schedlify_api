namespace SchedlifyApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using Repositories;
using DTO;

[ApiController]
[Route("classes")]
public class ClassesController : ControllerBase
{
    private readonly IClassRepository _repository;
    private readonly IGroupRepository _groupRepository;

    public ClassesController(
        IClassRepository repository,
        IGroupRepository groupRepository
    )
    {
        _repository = repository;
        _groupRepository = groupRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<ClassResponse>>> GetClassesByGroupId(
        [FromQuery] int groupId)
    {
        var group = await _groupRepository.GetById(groupId);
        if (group == null)
        {
            return BadRequest("Group not found");
        }

        var classes = await _repository.GetByGroupId(groupId);

        var response = classes.Select(c => new ClassResponse
        {
            Id = c.Id,
            Name = c.Name,
            GroupId = c.GroupId
        }).ToList();

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ClassResponse>> CreateClass(
        CreateClassRequest createClassRequest)
    {
        var group = await _groupRepository.GetById(createClassRequest.GroupId);
        if (group == null)
        {
            return BadRequest("Group not found");
        }

        var newClass = new Class
        {
            Name = createClassRequest.Name,
            GroupId = createClassRequest.GroupId
        };

        var createdClass = await _repository.Create(newClass);

        var response = new ClassResponse
        {
            Id = createdClass.Id,
            Name = createdClass.Name,
            GroupId = createdClass.GroupId
        };

        return CreatedAtAction(nameof(GetClassesByGroupId), new { groupId = response.GroupId }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ClassResponse>> EditClass(
        int id, EditClassRequest editClassRequest)
    {
        var existingClass = await _repository.GetByIdAsync(id);
        if (existingClass == null)
            return NotFound();

        var group = await _groupRepository.GetById(editClassRequest.GroupId);
        if (group == null)
            return BadRequest("Group not found");

        existingClass.Name = editClassRequest.Name;
        existingClass.GroupId = editClassRequest.GroupId;

        var updatedClass = await _repository.Update(existingClass);

        var response = new ClassResponse
        {
            Id = updatedClass.Id,
            Name = updatedClass.Name,
            GroupId = updatedClass.GroupId
        };

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClass(int id)
    {
        var existingClass = await _repository.GetByIdAsync(id);
        if (existingClass == null)
            return NotFound("Class not found");

        await _repository.Delete(id);
        return NoContent();
    }
}
