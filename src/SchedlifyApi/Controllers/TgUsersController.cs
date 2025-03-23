using SchedlifyApi.Attributes;
using SchedlifyApi.DTOs;
using SchedlifyApi.Models;
using SchedlifyApi.Repositories;

namespace SchedlifyApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using SchedlifyApi.DTOs;
using SchedlifyApi.Models;
using SchedlifyApi.Repositories;
using SchedlifyApi.Middleware;
using SchedlifyApi.Attributes;

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
    public async Task<ActionResult<TgUserResponse>> CreateTgUser(CreateTgUserRequest request, long telegramUid)
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
            CreatedAt = createdUser.CreatedAt
        };

        return CreatedAtAction(nameof(GetTgUser), new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<ActionResult<TgUserResponse>> GetTgUser(long telegramUid)
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
            CreatedAt = user.CreatedAt
        };

        return response;
    }
}