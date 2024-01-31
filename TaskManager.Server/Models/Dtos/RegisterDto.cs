using System.ComponentModel.DataAnnotations;

namespace TaskManager.Server.Models.Dtos;

public class RegisterDto
{
    [Required]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 character long")]
    // This regex is taken from my own code https://github.com/nayef-shuvo/SimpleBook/blob/master/Entities/Dtos/UserDto.cs
    // I hope this is not a violation
    [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^\da-zA-Z]).+$", ErrorMessage = "Password must contain at least one letter, one digit, and one special character.")]
    public string Password { get; set; }
}
