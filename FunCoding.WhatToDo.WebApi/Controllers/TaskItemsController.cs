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
    public async Task<IActionResult> GetTaskItems(int pageIndex = 1, int pageSize = 20)
    {
        var taskItems = await _taskItemRepo.GetTaskItemsAsync(pageIndex, pageSize);
        return Ok(taskItems);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTaskItem(Guid id)
    {
        var taskItem = await _taskItemRepo.GetTaskItemAsync(id);
        if (taskItem == null)
        {
            return NotFound();
        }
        return Ok(taskItem);
    }


    [HttpPost]
    public async Task<IActionResult> CreateTaskItem(TaskItem newTaskItem)
    {
        var taskItem = new TaskItem
        {
            Title = newTaskItem.Title,
            Description = newTaskItem.Description
        };
        await _taskItemRepo.CreateTaskItemAsync(taskItem);
        return CreatedAtAction(nameof(CreateTaskItem), new { id = taskItem.Id }, taskItem);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTaskItem(Guid id, TaskItem updateTaskItem)
    {
        var taskItem = await _taskItemRepo.UpdateTaskItemAsync(id, updateTaskItem);
        if (taskItem == null)
        {
            return NotFound();
        }
        return Ok(taskItem);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTaskItem(Guid id)
    {
        var taskItem = await _taskItemRepo.DeleteTaskItemAsync(id);
        if (taskItem == null)
        {
            return NotFound();
        }
        return Ok();
    }


}
