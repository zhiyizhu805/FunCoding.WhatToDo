using FunCoding.WhatToDo.WebApi.Data;
using FunCoding.WhatToDo.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FunCoding.WhatToDo.WebApi.Controllers;

[ApiController]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public TaskController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTasksAsync()
    {
        List<Models.Task> tasks = await _context.Tasks.ToListAsync();
        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTaskById([FromRoute] Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }
        return Ok(task);
    }


    [HttpPost]
    public async Task<IActionResult> CreateTask(string title, string description)
    {
        var task = new Models.Task
        {
            Title = title,
            Description = description
        };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(CreateTask), new { id = task.Id }, task);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTask(Guid id, string title, string description)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }
        task.Title = title;
        task.Description = description;
        await _context.SaveChangesAsync();
        return Ok(task);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTaskById(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return Ok();

    }


}
