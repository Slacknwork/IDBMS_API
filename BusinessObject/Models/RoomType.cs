﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models;

public class RoomType
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string ImageUrl { get; set; } = default!;

    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "money")]
    public decimal PricePerArea { get; set; }

    [Required]
    public double IsHidden { get; set; }

    public List<Room> Rooms { get; set; } = new();
}