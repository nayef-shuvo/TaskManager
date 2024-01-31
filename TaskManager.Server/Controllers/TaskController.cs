using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Server.Data;
using TaskManager.Server.Models;
using TaskManager.Server.Models.Dtos;
using Task = TaskManager.Server.Models.Task;

namespace TaskManager.Server.Controllers;

[ApiController]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TaskController> _logger;

    public TaskController(ApplicationDbContext context, ILogger<TaskController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = nameof(RoleType.User))]
    public async Task<IActionResult> GetTasks()
    {
        var claimedId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        _ = int.TryParse(claimedId, out int uid);

        var tasks = await _context.Tasks.Where(e => e.UserId == uid)
                                        .AsNoTracking().ToListAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}", Name = "GetTask")]
    [Authorize(Roles = nameof(RoleType.User))]
    public async Task<IActionResult> GetTask(int id)
    {
        var claimedId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        _ = int.TryParse(claimedId, out int uid);

        var task = await _context.Tasks.Where(e => e.UserId == uid)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => e.Id == id);

        if (task == null)
        {
            return BadRequest();
        }

        return Ok(task);
    }

    [HttpPost]
    [Authorize(Roles = nameof(RoleType.User))]
    public async Task<IActionResult> CreateTask([FromBody] TaskDto task)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var claimedId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        _ = int.TryParse(claimedId, out int uid);

        var newTask = new Task
        {
            UserId = uid,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status
        };

        await _context.Tasks.AddAsync(newTask);
        await _context.SaveChangesAsync();

        return CreatedAtRoute(nameof(GetTask), new {id = newTask.Id}, newTask);
    }
    
    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(RoleType.User))]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDto task)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var claimedId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        _ = int.TryParse(claimedId, out int uid);

        var taskExist = await _context.Tasks.FirstOrDefaultAsync(e => e.Id == id);
        if (taskExist == null)
        {
            return NotFound("Task not found");
        }

        if (uid != taskExist.UserId)
        {
            return BadRequest("You do not have the permission.");
        }

        taskExist.Title = task.Title;
        taskExist.Description = task.Description;
        taskExist.Status = task.Status;

        await _context.SaveChangesAsync();
        return Ok(taskExist);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(RoleType.User))]
    public async Task<IActionResult> RemoveTask(int id)
    {
        var claimedId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        _ = int.TryParse(claimedId, out int uid);

        var taskExist = await _context.Tasks.FirstOrDefaultAsync(e => e.Id == id);
        if (taskExist == null)
        {
            return NotFound("Task not found");
        }

        if (uid != taskExist.UserId)
        {
            return BadRequest("You do not have the permission.");
        }
        _context.Tasks.Remove(taskExist);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
