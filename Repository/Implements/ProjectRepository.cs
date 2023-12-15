﻿using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository.Implements
{
    public class ProjectRepository : IProjectRepository
    {
        public IEnumerable<Project> GetAll()
        {
            try
            {
                using var context = new IdtDbContext();
                return context.Projects
                    .Include(p => p.Transactions.Where(t => t.IsDeleted == false))
                    .Include(p => p.ProjectParticipations.Where(pp => pp.IsDeleted == false))
                    .Include(p => p.ProjectDocuments.Where(pd => pd.IsDeleted == false))
                    .OrderByDescending(time => time.CreatedDate)
                    .ToList();
            }
            catch
            {
                throw;
            }
        }

        public Project? GetById(Guid id)
        {
            try
            {
                using var context = new IdtDbContext();
                return context.Projects
                    .Include(pc => pc.ProjectCategory)
                    .Include(p => p.ProjectParticipations.Where(pp => pp.IsDeleted == false))
                        .ThenInclude(u => u.User)
                    .Include(p => p.Transactions.Where(t => t.IsDeleted == false))
                    .Include(p => p.ProjectDocuments.Where(pd => pd.IsDeleted == false))
                    .FirstOrDefault(project => project.Id == id);
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<Project> GetAdvertisementAllowedProjects()
        {
            try
            {
                using var context = new IdtDbContext();
                return context.Projects
                    .Include(p => p.ProjectParticipations.Where(pp => pp.IsDeleted == false))
                    //.Include(p => p.ProjectDocuments.Where(pd => pd.IsDeleted == false))
                    .Include(pc => pc.ProjectCategory)
                    .OrderByDescending(time => time.CreatedDate)
                    .Where(p => p.Status == ProjectStatus.Done && p.AdvertisementStatus != AdvertisementStatus.NotAllowed)
                    .ToList();
            }
            catch
            {
                throw;
            }
        }

        public Project? GetByIdWithSite(Guid id)
        {
            try
            {
                using var context = new IdtDbContext();
                return context.Projects
                    .Include(pc => pc.ProjectCategory)
                    .Include(pc => pc.Site)
                    .Include(p => p.ProjectParticipations.Where(pp => pp.IsDeleted == false))
                        .ThenInclude(u => u.User)
                    .Include(p => p.Transactions.Where(t => t.IsDeleted == false))
                    .Include(p => p.ProjectDocuments.Where(pd => pd.IsDeleted == false))
                    .FirstOrDefault(project => project.Id == id);
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<Project> GetBySiteId(Guid id)
        {
            try
            {
                using var context = new IdtDbContext();
                return context.Projects
                    .Include(p => p.Transactions.Where(t => t.IsDeleted == false))
                    .Include(p => p.ProjectParticipations.Where(pp => pp.IsDeleted == false))
                    .Include(p => p.ProjectDocuments.Where(pd => pd.IsDeleted == false))
                    .OrderByDescending(time => time.CreatedDate)
                    .Where(p => p.SiteId == id).ToList();
            }
            catch
            {
                throw;
            }
        }

        public Project Save(Project entity)
        {
            try
            {
                using var context = new IdtDbContext();
                var project = context.Projects.Add(entity);
                context.SaveChanges();
                return project.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(Project entity)
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
                var project = context.Projects.FirstOrDefault(project => project.Id == id);
                if (project != null)
                {
                    context.Projects.Remove(project);
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
