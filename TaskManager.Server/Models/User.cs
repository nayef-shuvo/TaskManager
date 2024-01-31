using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Server.Models;

public enum RoleType
{
    Admin,
    User
}

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; private init; }
    
    [Required]
    [MinLength(4, ErrorMessage = "Username must be at least 4 character long")]
    public required string Username { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required byte[] PasswordHash {get; set;}
    
    [Required]
    public required byte[] PasswordSalt {get; set;}

    [Required]
    public required RoleType Role {get; set;}
}
