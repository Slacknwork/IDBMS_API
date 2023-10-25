﻿using BusinessObject.Models;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository.Implements
{
    public class CommentRepository : ICommentRepository
    {
        public IEnumerable<Comment> GetAll()
        {
            try
            {
                using var context = new IdtDbContext();
                return context.Comments.ToList();
            }
            catch
            {
                throw;
            }
        }

        public Comment? GetById(Guid id)
        {
            try
            {
                using var context = new IdtDbContext();
                return context.Comments.FirstOrDefault(comment => comment.Id == id);
            }
            catch
            {
                throw;
            }
        }

        public Comment Save(Comment entity)
        {
            try
            {
                using var context = new IdtDbContext();
                var comment = context.Comments.Add(entity);
                context.SaveChanges();
                return comment.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(Comment entity)
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
                var comment = context.Comments.FirstOrDefault(comment => comment.Id == id);
                if (comment != null)
                {
                    context.Comments.Remove(comment);
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