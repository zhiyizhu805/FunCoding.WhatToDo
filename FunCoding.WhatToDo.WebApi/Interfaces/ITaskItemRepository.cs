using FunCoding.WhatToDo.WebApi.Models;

namespace FunCoding.WhatToDo.WebApi.Interfaces;

public interface ITaskItemRepository
{
    Task<List<TaskItem>> GetTasksAsync(int pageIndex, int pageSize);
    Task<TaskItem?> GetTaskByIdAsync(Guid id);
    Task<TaskItem> CreateTaskAsync(TaskItem newTaskItem);
    Task<TaskItem?> UpdateTaskAsync(Guid id, TaskItem updateTaskItem);
    Task<TaskItem?> DeleteTaskByIdAsync(Guid id);
}
