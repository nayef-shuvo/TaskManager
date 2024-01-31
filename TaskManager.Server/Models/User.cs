using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Server.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; private init; }
    
    [Required]
    [MinLength(4, ErrorMessage = "Username must be at least 4 character long")]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public byte[] PasswordHash {get; set;}
    
    [Required]
    public byte[] PasswordSalt {get; set;}
}
