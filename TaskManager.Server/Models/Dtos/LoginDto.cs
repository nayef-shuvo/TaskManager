using System.ComponentModel.DataAnnotations;

namespace TaskManager.Server.Models.Dtos;

public class LoginDto
{
    [Required]
    public required string Username { get; set; }

    [Required]    
    public required string Password { get; set; }
    
}
