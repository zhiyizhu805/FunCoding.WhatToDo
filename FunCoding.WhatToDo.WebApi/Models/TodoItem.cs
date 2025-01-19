using System.ComponentModel.DataAnnotations;

namespace FunCoding.WhatToDo.WebApi.Models;

public class TodoItem
{
    public long Id { get; set; }
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    public bool IsComplete { get; set; }

}
