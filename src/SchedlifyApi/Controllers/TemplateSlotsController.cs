//COPYRIGHT NIGGERCODE
namespace SchedlifyApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Models;
using Repositories;
using DTO;

[ApiController]
[Route("/template-slots")]
public class TemplateSlotsController : ControllerBase
{
    private readonly ITemplateSlotRepository _repository;

    public TemplateSlotsController(ITemplateSlotRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("by-department/{departmentId}")]
    public async Task<ActionResult<List<TemplateSlotDtos.Response>>> GetByDepartmentId(long departmentId)
    {
        var templateSlots = await _repository.GetByDepartmentIdAsync((int)departmentId);
        var response = templateSlots.Select(ts => new TemplateSlotDtos.Response
        {
            Id = ts.Id,
            DepartmentId = ts.DepartmentId,
            StartTime = ts.StartTime,
            EndTime = ts.EndTime,
            ClassNumber = ts.ClassNumber
        }).ToList();

        return Ok(response);
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> AddTemplateSlots([FromBody] TemplateSlotDtos.BulkCreateRequest request)
    {
        if (request == null || request.Slots == null || !request.Slots.Any())
        {
            return BadRequest("Invalid request body.");
        }

        var templateSlotsToCreate = request.Slots.OrderBy(s => s.StartTime).Select((slot, index) => new TemplateSlot
        {
            DepartmentId = request.DepartmentId,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            ClassNumber = index
        }).ToList();

        await _repository.CreateBulkAsync(request.DepartmentId, templateSlotsToCreate);

        return Ok("Template slots added successfully.");
    }
}