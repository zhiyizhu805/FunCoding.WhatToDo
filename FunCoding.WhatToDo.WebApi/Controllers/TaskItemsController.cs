using FunCoding.WhatToDo.WebApi.Data;
using FunCoding.WhatToDo.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FunCoding.WhatToDo.WebApi.Controllers;

[ApiController]
[Route("api/taskitems")]
public class TaskItemsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public TaskItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTasksAsync()
    {
        List<TaskItem> tasks = await _context.TaskItems.ToListAsync();
        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        var task = await _context.TaskItems.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }
        return Ok(task);
    }


    [HttpPost]
    public async Task<IActionResult> CreateTask(TaskItem newTaskItem)
    {
        var task = new TaskItem
        {
            Title = newTaskItem.Title,
            Description = newTaskItem.Description
        };
        _context.TaskItems.Add(task);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(CreateTask), new { id = task.Id }, task);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTask(TaskItem taskItem)
    {
        var task = await _context.TaskItems.FindAsync(taskItem.Id);
        if (task == null)
        {
            return NotFound();
        }
        task.Title = taskItem.Title;
        task.Description = taskItem.Description;
        task.Status = taskItem.Status;
        await _context.SaveChangesAsync();
        return Ok(task);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTaskById(Guid id)
    {
        var task = await _context.TaskItems.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        _context.TaskItems.Remove(task);
        await _context.SaveChangesAsync();
        return Ok();

    }


}
