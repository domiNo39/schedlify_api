
namespace SchedlifyApi.Controllers;

using Microsoft.AspNetCore.Mvc;

using Models;
using Repositories;
using Attributes;
using DTO;
using System.Text.RegularExpressions;


[ApiController]
[Route("/tgusers")]
[RequireTelegramUid]
public class TgUsersController : ControllerBase
{
    private readonly ITgUserRepository _repository;

    public TgUsersController(ITgUserRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<ActionResult<TgUserResponse>> CreateTgUser(
        [FromHeader(Name = "X-TG-UID")] long telegramUid,
        CreateTgUserRequest request
        )
    {

        // Check if user already exists by ID (Telegram UID)
        var existingUserById = await _repository.GetByIdAsync(telegramUid);
        if (existingUserById != null)
        {
            return Conflict("User with this Telegram ID already exists");
        }

        // Check if username is already taken
        if (!string.IsNullOrEmpty(request.Username))
        {
            var existingUserByUsername = await _repository.GetByUsernameAsync(request.Username);
            if (existingUserByUsername != null)
            {
                return Conflict("Username already exists");
            }
        }

        var user = new TgUser
        {
            Id = telegramUid,
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var createdUser = await _repository.CreateAsync(user);

        var response = new TgUserResponse
        {
            Id = createdUser.Id,
            Username = createdUser.Username,
            FirstName = createdUser.FirstName,
            LastName = createdUser.LastName,
            CreatedAt = createdUser.CreatedAt,
            GroupId = createdUser.GroupId,
            Subscribed = createdUser.Subscribed,
        };

        return CreatedAtAction(nameof(GetTgUser), new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<ActionResult<TgUserResponse>> GetTgUser([FromHeader(Name = "X-TG-UID")] long telegramUid)
    {

        var user = await _repository.GetByIdAsync(telegramUid);
        if (user == null)
        {
            return NotFound();
        }

        var response = new TgUserResponse
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            GroupId = user.GroupId,
            Subscribed = user.Subscribed,
        };

        return response;
    }
    [HttpPost("/select_group")]
    public async Task<ActionResult<TgUserResponse>> SelectGroup([FromHeader(Name = "X-TG-UID")] long telegramUid, int GroupId)
    {
        var user = await _repository.GetByIdAsync(telegramUid);
        if (user == null)
        {
            return NotFound();
        }
        user.GroupId = GroupId;
        await _repository.UpdateAsync(user);
        var response = new TgUserResponse
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            GroupId = user.GroupId,
            Subscribed = user.Subscribed,
        };
        return response;
    }
    [HttpPost("/change_subscription_status")]
    public async Task<ActionResult<TgUserResponse>> SelectGroup([FromHeader(Name = "X-TG-UID")] long telegramUid)
    {
        var user = await _repository.GetByIdAsync(telegramUid);
        if (user == null)
        {
            return NotFound();
        }
        user.Subscribed = !user.Subscribed;
        await _repository.UpdateAsync(user);
        var response = new TgUserResponse
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            GroupId = user.GroupId,
            Subscribed = user.Subscribed,
        };
        return response;
    }
}