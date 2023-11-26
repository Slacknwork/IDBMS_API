﻿using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.Request
{
    public class TaskDocumentRequest
    {
        [Required]
        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public string? Document { get; set; } = default!;

        [Required]
        public Guid TaskReportId { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
    }
}
