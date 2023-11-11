﻿using BusinessObject.Models;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implements
{
    public class ProjectDocumentRepository : IProjectDocumentRepository
    {
        public void DeleteById(Guid id)
        {
            try
            {
                using var context = new IdtDbContext();
                var pd = context.ProjectDocuments.Where(ctc => ctc.Id == id).FirstOrDefault();
                if (pd != null)
                {
                    context.ProjectDocuments.Remove(pd);
                    context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }

        }

        public IEnumerable<ProjectDocument> GetAll()
        {
            try
            {
                using var context = new IdtDbContext();
                return context.ProjectDocuments.ToList();
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<ProjectDocument> GetByFilter(Guid? projectId, Guid? TaskReportId, Guid? decorProgressReportId, int? documentTemplateId)
        {
            try
            {
                using var context = new IdtDbContext();
                return context.ProjectDocuments
                    .Where(pd =>
                        (projectId == null || pd.ProjectId == projectId) &&
                        (TaskReportId == null || pd.TaskReportId == TaskReportId) &&
                        (documentTemplateId == null || pd.ProjectDocumentTemplateId == documentTemplateId))
                    .ToList();
            }
            catch
            {
                throw;
            }
        }

        public ProjectDocument? GetById(Guid id)
        {
            try
            {
                using var context = new IdtDbContext();
                return context.ProjectDocuments.Where(pd => pd.Id == id).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public ProjectDocument? Save(ProjectDocument entity)
        {
            try
            {
                using var context = new IdtDbContext();
                var pd = context.ProjectDocuments.Add(entity);
                context.SaveChanges();
                return pd.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(ProjectDocument entity)
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
    }
}
