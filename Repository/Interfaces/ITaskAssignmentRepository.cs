﻿using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ITaskAssignmentRepository : ICrudBaseRepository<TaskAssignment, Guid>
    {
        IEnumerable<TaskAssignment?> GetByProjectId(Guid id);
        IEnumerable<TaskAssignment?> GetByUserId(Guid id);

    }
}