﻿using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implements
{
    public class ProjectTaskRepository : IProjectTaskRepository
    {
        public IEnumerable<ProjectTask> GetAll()
        {
            try
            {
                using var context = new IdtDbContext();
                return context.ProjectTasks
                    .Include(c => c.TaskCategory)
                    .ToList();
            }
            catch
            {
                throw;
            }
        }

        public ProjectTask? GetById(Guid id)
        {
            try
            {
                using var context = new IdtDbContext();
                return context.ProjectTasks
                    .Include(c => c.TaskCategory)
                    .Include(p => p.PaymentStage)
                    .Include(p => p.ParentTask)
                    .Include(i => i.InteriorItem)
                    .Include(r => r.Room)
                    .FirstOrDefault(task => task.Id == id);
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<ProjectTask?> GetByProjectId(Guid id)
        {
            try
            {
                using var context = new IdtDbContext();
                return context.ProjectTasks
                    .Include(c => c.TaskCategory)
                    .Include(p => p.PaymentStage)
                    .Include(i => i.InteriorItem)
                    .Include(r => r.Room)
                        .ThenInclude(f => f.Floor)
                    .Where(task => task.ProjectId == id)
                    .ToList();
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<ProjectTask?> GetByRoomId(Guid id)
        {
            try
            {
                using var context = new IdtDbContext();
                return context.ProjectTasks
                    .Include(c => c.TaskCategory)
                    .Include(p => p.PaymentStage)
                    .Include(i => i.InteriorItem)
                    .Include(r => r.Room)
                        .ThenInclude(f => f.Floor)
                    .Where(task => task.RoomId == id)
                    .ToList();
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<ProjectTask?> GetByPaymentStageId(Guid id)
        {
            try
            {
                using var context = new IdtDbContext();
                return context.ProjectTasks.Where(task => task.PaymentStageId == id).ToList();
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<ProjectTask?> GetSuggestionTasksByProjectId(Guid id)
        {
            try
            {
                using var context = new IdtDbContext();
                return context.ProjectTasks
                        .Include(c => c.TaskCategory)
                        .Include(p => p.PaymentStage)
                        .Include(i => i.InteriorItem)
                            .ThenInclude(c => c.InteriorItemCategory)
                        .Where(task => task.ProjectId == id
                            && task.InteriorItem != null
                            && task.InteriorItem.InteriorItemCategory != null
                            && (task.InteriorItem.InteriorItemCategory.InteriorItemType == BusinessObject.Enums.InteriorItemType.Furniture
                            || task.InteriorItem.InteriorItemCategory.InteriorItemType == BusinessObject.Enums.InteriorItemType.CustomFurniture))
                        .ToList();
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<ProjectTask?> GetSuggestionTasksByRoomId(Guid id)
        {
            try
            {
                using var context = new IdtDbContext();
                return context.ProjectTasks
                        .Include(c => c.TaskCategory)
                        .Include(p => p.PaymentStage)
                        .Include(i => i.InteriorItem)
                            .ThenInclude(c => c.InteriorItemCategory)
                        .Where(task => task.RoomId == id
                            && task.InteriorItem != null
                            && task.InteriorItem.InteriorItemCategory != null
                            && (task.InteriorItem.InteriorItemCategory.InteriorItemType == BusinessObject.Enums.InteriorItemType.Furniture
                            || task.InteriorItem.InteriorItemCategory.InteriorItemType == BusinessObject.Enums.InteriorItemType.CustomFurniture))
                        .ToList();
            }
            catch
            {
                throw;
            }
        }

        public ProjectTask Save(ProjectTask entity)
        {
            try
            {
                using var context = new IdtDbContext();
                var task = context.ProjectTasks.Add(entity);
                context.SaveChanges();
                return task.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(ProjectTask entity)
        {
            try
            {
                using var context = new IdtDbContext();
                context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void DeleteById(Guid id)
        {
            try
            {
                using var context = new IdtDbContext();
                var task = context.ProjectTasks.FirstOrDefault(task => task.Id == id);
                if (task != null)
                {
                    context.ProjectTasks.Remove(task);
                    context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
