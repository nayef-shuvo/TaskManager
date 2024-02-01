using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Server.Data;
using TaskManager.Server.Models;
using TaskManager.Server.Models.Dtos;

namespace TaskManager.Server.Controllers;

[ApiController]
[Route("api")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public UserController(ApplicationDbContext context, IConfiguration config, ILogger<UserController> logger)
    {
        _logger = logger;
        _context = context;
        _config = config;
    }


    // for testing purposes
    // [Authorize(Roles = nameof(RoleType.User))]
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        _logger.LogInformation(nameof(RoleType.User));

        _logger.LogInformation(string.Join("\n", User.Claims));


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

        //* checking strong password

        if (!IsPasswordStrong(request.Password, out string errorMessage))
        {
            return BadRequest(errorMessage);
        }


        //* everything ok
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

        return Ok(new { Message = "User registration has been completed successfully" });
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

        var jwt = GenerateJwtToken(user);
        return Ok(new
        {
            user.Username,
            user.Email,
            user.Role,
            Bearer = jwt
        });
    }

    private (byte[], byte[]) GenerateHashAndSalt(string password)
    {
        using var hmac = new HMACSHA512();
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return (hash, hmac.Key);
    }

    private bool VerifyPassword(string password, byte[] hash, byte[] salt)
    {
        using var hmac = new HMACSHA512(salt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(hash);
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var claims = new Claim[]
        {
            new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool IsPasswordStrong(string password, out string errorMessage)
    {
        char[] specialCharacters = "~`!@#$%^&*()-_=+:;,.<>/?".ToCharArray();
        int[] digits = Enumerable.Range('0', 10).ToArray();
        int[] capitalLetters = Enumerable.Range('A', 26).ToArray();
        int[] smallLetters = Enumerable.Range('a', 26).ToArray();

        bool hasSpecialCharacter = false;
        bool hasDigit = false;
        bool hasSmallLetter = false;
        bool hasCapitalLetter = false;

        foreach (var e in password)
        {
            if (specialCharacters.Contains(e)) hasSpecialCharacter = true;
            else if (digits.Contains(e)) hasDigit = true;
            else if (capitalLetters.Contains(e)) hasCapitalLetter = true;
            else if (smallLetters.Contains(e)) hasSmallLetter = true;
            else
            {
                errorMessage = $"Only these special characters are allowed {specialCharacters}";
                return false;
            }
        }
        var isOk = hasSpecialCharacter & hasDigit & hasCapitalLetter & hasSmallLetter;

        errorMessage = isOk ? string.Empty :
                    "Password must contain at least a capital letter, a small letter, a digit, and a special character";
        return isOk;
    }

}
