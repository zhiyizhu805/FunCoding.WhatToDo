using FunCoding.WhatToDo.WebApi.Data;
using FunCoding.WhatToDo.WebApi.Interfaces;
using FunCoding.WhatToDo.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FunCoding.WhatToDo.WebApi.repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly ApplicationDbContext _context;
    public TaskItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public Task<List<TaskItem>> GetTaskItemsAsync(int pageIndex, int pageSize)
    {
        return _context.TaskItems.OrderBy(t => t.CreatedAt).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<TaskItem?> GetTaskItemAsync(Guid id)
    {
        return await _context.TaskItems.FindAsync(id);
    }

    public async Task<TaskItem> CreateTaskItemAsync(TaskItem taskItem)
    {
        await _context.TaskItems.AddAsync(taskItem);
        await _context.SaveChangesAsync();
        return taskItem;
    }

    public async Task<TaskItem?> UpdateTaskItemAsync(Guid id, TaskItem updateTaskItem)
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

    public async Task<TaskItem?> DeleteTaskItemAsync(Guid id)
    {
        var taskItem = await _context.TaskItems.FindAsync(id);
        if (taskItem == null)
            return null;
        _context.TaskItems.Remove(taskItem);
        await _context.SaveChangesAsync();
        return taskItem;
    }
}
