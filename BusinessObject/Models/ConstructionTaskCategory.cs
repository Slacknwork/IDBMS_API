﻿using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models;

public class ConstructionTaskCategory
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    [Required]
    public bool IsDeleted { get; set; }

    public List<ConstructionTask> ConstructionTasks { get; set; } = new();
}