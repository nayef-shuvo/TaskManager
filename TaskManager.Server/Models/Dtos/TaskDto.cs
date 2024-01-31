using System.ComponentModel.DataAnnotations;

namespace TaskManager.Server.Models.Dtos;

public class TaskDto
{   
    [Required]    
    public required string Title { get; set; }
    
    [Required]
    public required string Description { get; set; }

    [Required]
    public bool Status { get; set; }
}
