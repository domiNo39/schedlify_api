using Microsoft.AspNetCore.Mvc;
using SchedlifyApi.DTO;
using SchedlifyApi.Models;
using SchedlifyApi.Repositories;

using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;

    public UsersController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(RegisterUserRequest registerUserRequest)
    {
        // Check if the user already exists
        var existingUser = _repository.GetByLogin(registerUserRequest.Login);
        if (existingUser != null)
        {
            return Conflict("User with this login already exists.");
        }
        
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerUserRequest.Password);

        // Create new user
        var newUser = new User
        {
            Login = registerUserRequest.Login,
            PasswordHash = passwordHash // Consider hashing this before saving
        };

        var createdUser = _repository.Add(newUser);

        var response = new UserResponse
        {
            Id = createdUser.Id,
            Login = createdUser.Login
        };

        return CreatedAtAction(nameof(GetUserById), new { userId = response.Id }, response);
    }

    [HttpPost("login")]
    public ActionResult<UserResponse> Login(LoginUserRequest loginRequest)
    {
        var user = _repository.GetByLogin(loginRequest.Login);
        if (user == null) // Add password hashing check in real cases
        {
            return Unauthorized("Invalid credentials.");
        }
        if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid credentials.");
        }


        var response = new UserResponse
        {
            Id = user.Id,
            Login = user.Login
        };

        return Ok(response);
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<UserResponse>> GetUserById(int userId)
    {
        var user = await _repository.GetUserById(userId);
        if (user == null)
            return NotFound();

        var response = new UserResponse
        {
            Id = user.Id,
            Login = user.Login
        };

        return Ok(response);
    }
}
