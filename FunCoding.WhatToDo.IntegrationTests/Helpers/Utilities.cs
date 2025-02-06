using FunCoding.WhatToDo.WebApi.Data;
using FunCoding.WhatToDo.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FunCoding.WhatToDo.IntegrationTests.Helpers;

public static class Utilities
{
    public static void InitializeDatabase(ApplicationDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        SeedDatabase(context);
    }

    public static void Reset(ApplicationDbContext context)
    {
        context.TaskItems.ExecuteDelete();
        context.SaveChanges();
        SeedDatabase(context);
    }

    public static void Cleanup(ApplicationDbContext context)
    {
        context.TaskItems.ExecuteDelete();
        context.SaveChanges();
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
            },
            new()
            {
                Id = Guid.Parse("798a4706-4c51-48c9-8310-531d7364c927"),
                Title = "Sing a song",
                Description = "Sing a song for at least 20 mins.",
            },
            new()
            {
                Id = Guid.Parse("798a4706-4c51-48c9-8310-531d7364c928"),
                Title = "Do house cleaning",
                Description = "Do house cleaning at least 20 mins.",
            },
            new()
            {
                Id = Guid.Parse("798a4706-4c51-48c9-8310-531d7364c910"),
                Title = "Read a book with your children",
                Description = "Read a book with your children for at least 20 mins.",
            },
            new()
            {
                Id = Guid.Parse("798a4706-4c51-48c9-8310-531d7364c911"),
                Title = "Play with your cats",
                Description = "Play with your cats for at least 20 mins.",
            },
            new()
            {
                Id = Guid.Parse("798a4706-4c51-48c9-8310-531d7364c912"),
                Title = "Wash dishes",
                Description = "Wash dishes for at least 20 mins.",
            },
            new()
            {
                Id = Guid.Parse("798a4706-4c51-48c9-8310-531d7364c913"),
                Title = "Do garden work",
                Description = "Do garden work for at least 20 mins.",
            }
        };
        context.TaskItems.AddRange(taskItems);
        context.SaveChanges();
    }

}
