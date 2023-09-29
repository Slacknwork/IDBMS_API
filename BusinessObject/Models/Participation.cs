﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models;

public class Participation
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    public bool IsDeleted { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = default!;

    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = default!;
}
