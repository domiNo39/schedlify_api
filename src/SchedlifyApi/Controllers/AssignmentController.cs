using System.Runtime.InteropServices.JavaScript;

namespace SchedlifyApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using Repositories;
using DTO;

[ApiController]
[Route("assignments")]
public class AssignmentController : ControllerBase
{
    private readonly IAssignmentRepository _repository;
    private readonly ITemplateSlotRepository _templateSlotRepository;
    private readonly IClassRepository _classRepository;

    public AssignmentController(
        IAssignmentRepository repository,
        ITemplateSlotRepository templateSlotRepository,
        IClassRepository classRepository
    )
    {
        _repository = repository;
        _templateSlotRepository = templateSlotRepository;
        _classRepository = classRepository;
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<List<AssignmentResponse>>> GetById(int id)
    {
        var assignment = await _repository.GetByIdAsync(id);
        if (assignment == null)
        {
            return NotFound("Assignment not found");
        }
        var response = new AssignmentResponse
        {
            Id = assignment.Id,
            GroupId = assignment.GroupId,
            ClassId = assignment.ClassId,
            Weekday = assignment.Weekday,
            StartTime = assignment.StartTime,
            Type = assignment.Type,
            Lecturer = assignment.Lecturer,
            Address = assignment.Address,
            RoomNumber = assignment.RoomNumber,
            ClassType = assignment.ClassType,
            Mode = assignment.Mode,
            Date = assignment.Date,
            EndTime = assignment.EndTime
        };
        return Ok(response);
    }

    [HttpGet("/iseveneweek")]
    public static bool IsEvenWeek(DateOnly date)
    {
        int weekOfYear = (date.DayOfYear - 1) / 7 + 1;
        return weekOfYear % 2 == 0;
    }

    [ApiExplorerSettings(IgnoreApi = true)] // костиль єбаний
    public async Task<List<Assignment>> GetByGroupIdAndDateInner(int groupId, DateOnly date)
    {
        int dayOfWeek = (int)date.DayOfWeek; 
        List<Assignment> regularAssignments = await _repository.GetAssignmentsByWeekday(groupId, (Weekday)((dayOfWeek + 6) % 7), AssignmentType.Regular);
        List<Assignment> intervalAssignments;
        if (IsEvenWeek(date))
        {
            intervalAssignments = await _repository.GetAssignmentsByWeekday(groupId, (Weekday)((dayOfWeek + 6) % 7), AssignmentType.Even);
        }
        else
        {
            intervalAssignments = await _repository.GetAssignmentsByWeekday(groupId, (Weekday)((dayOfWeek + 6) % 7), AssignmentType.Odd);
        }
        List<Assignment> specialAssignments = await _repository.GetAssignmentsByDate(groupId, date);
        regularAssignments.RemoveAll(assignment =>specialAssignments.Any(p => p.StartTime == assignment.StartTime));
        List<Assignment> responseAssignments = regularAssignments.Concat(specialAssignments).Concat(intervalAssignments).OrderBy(p => p.StartTime)
            .ToList();
        return responseAssignments;

    }
    
    [HttpGet("by_group_id_and_date")]
    public async Task<ActionResult<List<AssignmentResponse>>> GetByGroupIdAndDate(int groupId, DateOnly date)
    {
        List<Assignment> responseAssignments = await GetByGroupIdAndDateInner(groupId, date);
        var response = responseAssignments.Select(assignment => new AssignmentResponse
        {
            Id = assignment.Id,
            GroupId = assignment.GroupId,
            ClassId = assignment.ClassId,
            Weekday = assignment.Weekday,
            StartTime = assignment.StartTime,
            Type = assignment.Type,
            Lecturer = assignment.Lecturer,
            Address = assignment.Address,
            RoomNumber = assignment.RoomNumber,
            ClassType = assignment.ClassType,
            Mode = assignment.Mode,
            Date = assignment.Date,
            EndTime = assignment.EndTime
        }).ToList();
        return Ok(response);
    }
    
    [HttpPost("regular")]
    public async Task<ActionResult<AssignmentResponse>> AddRegularAssignment(CreateRegularAssignmentRequest request)
    {   
        var class_ = await _classRepository.GetByIdAsync(request.ClassId);
        if (class_ == null)
        {
            return BadRequest("Class not found");
        }
        TemplateSlot? slot = await _templateSlotRepository.GetByDepartmentIdAndClassNumber(class_.Group.DepartmentId, request.ClassNumber);
        if (slot is null)
        {
            return BadRequest("Slot not found");
        }
        Assignment assignment = new Assignment{
            GroupId = class_.GroupId,
            ClassId = request.ClassId,
            Weekday = request.Weekday,
            StartTime = slot.StartTime,
            Type = AssignmentType.Regular,
            Lecturer = request.Lecturer,
            Address = request.Address,
            RoomNumber = request.RoomNumber,
            ClassType = request.ClassType,
            Mode = request.Mode,
            Date = null,
            EndTime = slot.EndTime};
        assignment = await _repository.AddAssignment(assignment);
        
        var response = new AssignmentResponse
        {
            Id = assignment.Id,
            GroupId = assignment.GroupId,
            ClassId = assignment.ClassId,
            Weekday = assignment.Weekday,
            StartTime = assignment.StartTime,
            Type = assignment.Type,
            Lecturer = assignment.Lecturer,
            Address = assignment.Address,
            RoomNumber = assignment.RoomNumber,
            ClassType = assignment.ClassType,
            Mode = assignment.Mode,
            Date = assignment.Date,
            EndTime = assignment.EndTime
        };
        return response;
    }

    [HttpPost("interval")]
    public async Task<ActionResult<Assignment>> AddIntervalAssignment(CreateIntervalAssignmentRequest request)
    {
        var class_ = await _classRepository.GetByIdAsync(request.ClassId);
        if (class_ == null)
        {
            return BadRequest("Class not found");
        }

        TemplateSlot? slot = await _templateSlotRepository.GetByDepartmentIdAndClassNumber(class_.Group.DepartmentId, request.ClassNumber);
        if (slot is null)
        {
            return BadRequest("Slot not found");
        }
        Assignment assignment = new Assignment{
            GroupId = class_.GroupId,
            ClassId = request.ClassId,
            Weekday = request.Weekday,
            StartTime = slot.StartTime,
            Type = request.Type,
            Lecturer = request.Lecturer,
            Address = request.Address,
            RoomNumber = request.RoomNumber,
            ClassType = request.ClassType,
            Mode = request.Mode,
            Date = null,
            EndTime = slot.EndTime};
        assignment = await _repository.AddAssignment(assignment);
        
        var response = new AssignmentResponse
        {
            Id = assignment.Id,
            GroupId = assignment.GroupId,
            ClassId = assignment.ClassId,
            Weekday = assignment.Weekday,
            StartTime = assignment.StartTime,
            Type = assignment.Type,
            Lecturer = assignment.Lecturer,
            Address = assignment.Address,
            RoomNumber = assignment.RoomNumber,
            ClassType = assignment.ClassType,
            Mode = assignment.Mode,
            Date = assignment.Date,
            EndTime = assignment.EndTime
        };
        return Ok(response);
    }

    [HttpPost("special")]
    public async Task<ActionResult<AssignmentResponse>> AddSpecialAssignment(CreateSpecialAssignmentRequest request)
    {
        var class_ = await _classRepository.GetByIdAsync(request.ClassId);
        if (class_ == null)
        {
            return BadRequest("Class not found");
        }

        TemplateSlot? slot = await _templateSlotRepository.GetByDepartmentIdAndClassNumber(class_.Group.DepartmentId, request.ClassNumber);
        if (slot is null)
        {
            return BadRequest("Slot not found");
        }
        Assignment assignment = new Assignment{
            GroupId = class_.GroupId,
            ClassId = request.ClassId,
            Weekday = (Weekday)(((int)request.Date.DayOfWeek + 6) % 7),
            StartTime = slot.StartTime,
            Type = AssignmentType.Special,
            Lecturer = request.Lecturer,
            Address = request.Address,
            RoomNumber = request.RoomNumber,
            ClassType = request.ClassType,
            Mode = request.Mode,
            Date = request.Date,
            EndTime = slot.EndTime};
        assignment = await _repository.AddAssignment(assignment);
        
        var response = new AssignmentResponse
        {
            Id = assignment.Id,
            GroupId = assignment.GroupId,
            ClassId = assignment.ClassId,
            Weekday = assignment.Weekday,
            StartTime = assignment.StartTime,
            Type = assignment.Type,
            Lecturer = assignment.Lecturer,
            Address = assignment.Address,
            RoomNumber = assignment.RoomNumber,
            ClassType = assignment.ClassType,
            Mode = assignment.Mode,
            Date = assignment.Date,
            EndTime = assignment.EndTime
        };
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClass(int id)
    {
        var existingClass = await _repository.GetByIdAsync(id);
        if (existingClass == null)
            return NotFound("Assignment not found");

        await _repository.DeleteAssignment(id);
        return NoContent();
    }
}