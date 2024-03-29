﻿using BusinessObject.Models;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implements
{
    public class BookingRequestRepository : IBookingRequestRepository
    {
        public void DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BookingRequest> GetAll()
        {
            try
            {
                using var context = new IdtDbContext();
                return context.BookingRequests
                    .Where(br => br.IsDeleted == false)
                    .OrderByDescending(br => br.UpdatedDate ?? br.CreatedDate)
                    .ToList();
            }
            catch
            {
                throw;
            }
        }

        public BookingRequest? GetById(Guid id)
        {
            try
            {
                using var context = new IdtDbContext();
                return context.BookingRequests
                    .Where(br => br.Id == id && br.IsDeleted == false)
                    .FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public BookingRequest? Save(BookingRequest entity)
        {
            try
            {
                using var context = new IdtDbContext();
                var BookingRequest = context.BookingRequests.Add(entity);
                context.SaveChanges();
                return BookingRequest.Entity;
            }
            catch
            {
                throw;
            }
        }

        public void Update(BookingRequest entity)
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
