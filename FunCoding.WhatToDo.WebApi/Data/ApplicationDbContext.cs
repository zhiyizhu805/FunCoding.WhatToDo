namespace FunCoding.WhatToDo.WebApi.Data;
using Microsoft.EntityFrameworkCore;
using Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems { get; set; } = null!;
}
