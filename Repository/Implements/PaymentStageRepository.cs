﻿using BusinessObject.Models;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository.Implements
{
    public class PaymentStageRepository : IPaymentStageRepository
    {
        public IEnumerable<PaymentStage> GetAll()
        {
            try
            {
                using var context = new IdtDbContext();
                return context.PaymentStages
                    .OrderBy(ps => ps.StageNo)
                    .Where(stage => stage.IsDeleted == false)
                    .ToList();
            }
            catch
            {
                throw;
            }
        }

        public PaymentStage? GetById(Guid id)
        {
            try
            {
                using var context = new IdtDbContext();
                return context.PaymentStages.FirstOrDefault(stage => stage.Id == id && stage.IsDeleted == false);
            }
            catch
            {
                throw;
            }
        }
        public IEnumerable<PaymentStage> GetByProjectId(Guid id)
        {
            try
            {
                using var context = new IdtDbContext();
                return context.PaymentStages
                         .Where(stage => stage.ProjectId == id && stage.IsDeleted == false)
                         .OrderBy(stage => stage.StageNo)
                         .ToList();
            }
            catch
            {
                throw;
            }
        }        
        
        public PaymentStage? GetByStageNoByProjectId(int no, Guid projectId)
        {
            try
            {
                using var context = new IdtDbContext();
                return context.PaymentStages.FirstOrDefault(stage => stage.StageNo == no && stage.ProjectId == projectId && stage.IsDeleted == false);
            }
            catch
            {
                throw;
            }
        }

        public PaymentStage Save(PaymentStage entity)
        {
            try
            {
                using var context = new IdtDbContext();
                var stage = context.PaymentStages.Add(entity);
                context.SaveChanges();
                return stage.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(PaymentStage entity)
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
                var stage = context.PaymentStages.FirstOrDefault(stage => stage.Id == id);
                if (stage != null)
                {
                    context.PaymentStages.Remove(stage);
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
