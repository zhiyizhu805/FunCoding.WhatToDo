using System.ComponentModel.DataAnnotations;
using FunCoding.WhatToDo.WebApi.Enums;

namespace FunCoding.WhatToDo.WebApi.Models;


public class Task
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(255)]
    public string? Description { get; set; }
    public Status Status { get; set; } = Status.New;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

}
