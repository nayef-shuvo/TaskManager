using Microsoft.AspNetCore.Mvc;
using TaskManager.Server.Data;

namespace TaskManager.Server.Controllers;

[ApiController]
[Route("api")]
public class TaskController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TaskController(ApplicationDbContext context)
    {
        _context = context;
    }
    
}
