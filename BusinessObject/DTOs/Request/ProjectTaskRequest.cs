﻿using BusinessObject.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.Request
{
    public class ProjectTaskRequest
    {
        [Required]
        public string Code { get; set; } = default!;

        [Required]
        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Percentage { get; set; }

        [Required]
        public CalculationUnit CalculationUnit { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal PricePerUnit { get; set; }

        [Required]
        public double UnitInContract { get; set; }

        [Required]
        public double UnitUsed { get; set; }

        [Required]
        public bool IsIncurred { get; set; }

        [Required]
        public DateTime StartedDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? NoDate { get; set; }

        public Guid? ParentTaskId { get; set; }

        public int? TaskCategoryId { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public Guid PaymentStageId { get; set; }

        public Guid? InteriorItemId { get; set; }

        [Required]
        public int TaskDesignId { get; set; }

        public Guid? RoomId { get; set; }

        [Required]
        public ProjectTaskStatus Status { get; set; }
    }
}