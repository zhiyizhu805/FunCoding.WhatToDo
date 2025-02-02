using FunCoding.WhatToDo.WebApi.Data;
using FunCoding.WhatToDo.WebApi.Interfaces;
using FunCoding.WhatToDo.WebApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FunCoding.WhatToDo.WebApi.Repository;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly ApplicationDbContext _context;
    public TaskItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<TaskItem>> GetTasksAsync(int pageIndex, int pageSize)
    {
        return await _context.TaskItems.OrderBy(t => t.CreatedAt).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(Guid id)
    {
        return await _context.TaskItems.FindAsync(id);
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem taskItem)
    {
        _context.TaskItems.Add(taskItem);
        await _context.SaveChangesAsync();
        return taskItem;
    }

    public async Task<TaskItem?> UpdateTaskAsync(Guid id, TaskItem updateTaskItem)
    {
        var taskItem = await _context.TaskItems.FindAsync(id);
        if (taskItem == null)
            return null;
        taskItem.Title = updateTaskItem.Title;
        taskItem.Description = updateTaskItem.Description;
        taskItem.Status = updateTaskItem.Status;
        await _context.SaveChangesAsync();
        return taskItem;
    }

    public async Task<TaskItem?> DeleteTaskByIdAsync(Guid id)
    {
        var taskItem = await _context.TaskItems.FindAsync(id);
        if (taskItem == null)
            return null;
        _context.TaskItems.Remove(taskItem);
        await _context.SaveChangesAsync();
        return taskItem;
    }
}
