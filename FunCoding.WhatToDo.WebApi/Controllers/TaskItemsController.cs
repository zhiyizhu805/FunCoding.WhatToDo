using FunCoding.WhatToDo.WebApi.Data;
using FunCoding.WhatToDo.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FunCoding.WhatToDo.WebApi.Controllers;

[ApiController]
[Route("api/taskItems")]
public class TaskItemsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public TaskItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasksAsync(int pageIndex = 1, int pageSize = 20)
    {
        var taskItems = await _context.TaskItems.OrderBy(t => t.CreatedAt).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(taskItems);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        var taskItem = await _context.TaskItems.FindAsync(id);
        if (taskItem == null)
        {
            return NotFound();
        }
        return Ok(taskItem);
    }


    [HttpPost]
    public async Task<IActionResult> CreateTask(TaskItem newTaskItem)
    {
        var taskItem = new TaskItem
        {
            Title = newTaskItem.Title,
            Description = newTaskItem.Description
        };
        _context.TaskItems.Add(taskItem);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(CreateTask), new { id = taskItem.Id }, taskItem);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTask(Guid id,TaskItem updateTaskItem)
    {
        var taskItem = await _context.TaskItems.FindAsync(id);
        if (taskItem == null)
        {
            return NotFound();
        }
        taskItem.Title = updateTaskItem.Title;
        taskItem.Description = updateTaskItem.Description;
        taskItem.Status = updateTaskItem.Status;
        await _context.SaveChangesAsync();
        return Ok(taskItem);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTaskById(Guid id)
    {
        var taskItem = await _context.TaskItems.FindAsync(id);
        if (taskItem == null)
        {
            return NotFound();
        }

        _context.TaskItems.Remove(taskItem);
        await _context.SaveChangesAsync();
        return Ok();

    }


}
