using FunCoding.WhatToDo.WebApi.Models;

namespace FunCoding.WhatToDo.WebApi.Interfaces;

public interface ITaskItemRepository
{
    Task<List<TaskItem>> GetTaskItemsAsync(int pageIndex, int pageSize);
    Task<TaskItem?> GetTaskItemAsync(Guid id);
    Task<TaskItem> CreateTaskItemAsync(TaskItem newTaskItem);
    Task<TaskItem?> UpdateTaskItemAsync(Guid id, TaskItem updateTaskItem);
    Task<TaskItem?> DeleteTaskItemAsync(Guid id);
}
