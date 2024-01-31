using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Server.Data;
using TaskManager.Server.Models;
using TaskManager.Server.Models.Dtos;

namespace TaskManager.Server.Controllers;

[ApiController]
[Route("api")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }


    // for testing purposes
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }
    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        _context.Users.Remove(user);


        await _context.SaveChangesAsync();
        return Ok(user);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userExist = await _context.Users.FirstOrDefaultAsync(e => e.Username == request.Username);
        if (userExist != null)
        {
            return BadRequest("Username already exists");
        }
        var emailExist = await _context.Users.FirstOrDefaultAsync(e => e.Email == request.Email);
        if (emailExist != null)
        {
            return BadRequest("Email already exists");
        }

        var (hash, salt) = GenerateHashAndSalt(request.Password);
        var user = new User 
        { 
            Username = request.Username, 
            Email = request.Email, 
            PasswordHash = hash, 
            PasswordSalt = salt,
            Role = RoleType.User
        };
        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(e => e.Username == request.Username);
        if (user == null)
        {
            return BadRequest("Invalid username or password");
        }
        var isPasswordCorrect = VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);
        if (!isPasswordCorrect) 
        {
            return BadRequest("Invalid username or password");
        }
        return Ok(user.Username);
    }

    (byte[], byte[]) GenerateHashAndSalt(string password)
    {
        using var hmac = new HMACSHA512();
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return (hash, hmac.Key);
    }

    bool VerifyPassword(string password, byte[] hash, byte[] salt)
    {
        using var hmac = new HMACSHA512(salt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(hash);
    }


}
