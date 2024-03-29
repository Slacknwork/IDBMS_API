﻿using BusinessObject.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDBMS_API.DTOs.Request
{
    public class TaskReportRequest
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public double UnitUsed { get; set; }

        public string? Description { get; set; }

        [Required]
        public Guid ProjectTaskId { get; set; }
        public List<TaskDocumentRequest>? documentList { get; set; }
    }
}
