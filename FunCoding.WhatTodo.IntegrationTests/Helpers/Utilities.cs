using FunCoding.WhatToDo.WebApi.Data;
using FunCoding.WhatToDo.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FunCoding.WhatTodo.IntegrationTests.Helpers;

public static class Utilities
{
    public static void InitializeDatabase(ApplicationDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        SeedDatabase(context);
    }

    public static void Cleanup(ApplicationDbContext context)
    {
        context.TaskItems.ExecuteDelete();
        context.SaveChanges();
        SeedDatabase(context);
    }

    private static void SeedDatabase(ApplicationDbContext context)
    {
        var taskItems = new List<TaskItem>
        {
            new()
            {
                Id = Guid.Parse("8a9de219-2dde-4f2a-9ebd-b1f8df9fef03"),
                Title = "Read a book",
                Description = "Read a book everyday and write some notes about it.",
            },
            new()
            {
                Id = Guid.Parse("798a4706-4c51-48c9-8310-531d7364c926"),
                Title = "Have a walk",
                Description = "Have a walk everyday for at least 20 mins.",
            }
        };
        context.TaskItems.AddRange(taskItems);
        context.SaveChanges();
    }

}
