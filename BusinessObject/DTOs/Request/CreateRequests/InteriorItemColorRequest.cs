﻿using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.Request.CreateRequests
{
    public class InteriorItemColorRequest
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public ColorType Type { get; set; }

        [Required]
        public string PrimaryColor { get; set; } = default!;

        public string? SecondaryColor { get; set; }
    }
}
