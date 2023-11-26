﻿using BusinessObject.Models;
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
                return context.ProjectTasks.ToList();
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
                return context.ProjectTasks.FirstOrDefault(task => task.Id == id);
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
                return context.ProjectTasks.Where(task => task.ProjectId == id).ToList();
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
