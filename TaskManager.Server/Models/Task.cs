using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Server.Models;

public class Task
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; private init; }

    [Required]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    
    [Required]    
    public string Title { get; set; }
    
    public string Description { get; set; }

    public bool Status { get; set; }
}
