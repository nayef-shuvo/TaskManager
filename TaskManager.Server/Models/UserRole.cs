using System.ComponentModel.DataAnnotations;

namespace TaskManager.Server.Models;

public enum Role
{
    Admin,
    User
}

public class UserRole
{
    // User Id
    [Key]
    [Required]
    public int Id { get; set; }
    
    [Required]
    public Role Role { get; set; }
}
