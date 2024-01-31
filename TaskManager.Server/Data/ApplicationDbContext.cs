using Microsoft.EntityFrameworkCore;
using TaskManager.Server.Models;
using Task = TaskManager.Server.Models.Task;

namespace TaskManager.Server.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Task> Tasks { get; set; }
}
