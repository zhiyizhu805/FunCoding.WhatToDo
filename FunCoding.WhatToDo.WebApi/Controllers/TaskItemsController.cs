using FunCoding.WhatToDo.WebApi.Interfaces;
using FunCoding.WhatToDo.WebApi.Models;
using Microsoft.AspNetCore.Mvc;


namespace FunCoding.WhatToDo.WebApi.Controllers;

[ApiController]
[Route("api/taskItems")]
public class TaskItemsController : ControllerBase
{
    private readonly ITaskItemRepository _taskItemRepo;
    public TaskItemsController(ITaskItemRepository taskItemRepo)
    {
        _taskItemRepo = taskItemRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks(int pageIndex = 1, int pageSize = 20)
    {
        var taskItems = await _taskItemRepo.GetTasksAsync(pageIndex, pageSize);
        return Ok(taskItems);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        var taskItem = await _taskItemRepo.GetTaskByIdAsync(id);
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
        await _taskItemRepo.CreateTaskAsync(taskItem);
        return CreatedAtAction(nameof(CreateTask), new { id = taskItem.Id }, taskItem);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTask(Guid id, TaskItem updateTaskItem)
    {
        var taskItem = await _taskItemRepo.UpdateTaskAsync(id, updateTaskItem);
        if (taskItem == null)
        {
            return NotFound();
        }
        return Ok(taskItem);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTaskById(Guid id)
    {
        var taskItem = await _taskItemRepo.DeleteTaskByIdAsync(id);
        if (taskItem == null)
        {
            return NotFound();
        }
        return Ok();
    }


}
