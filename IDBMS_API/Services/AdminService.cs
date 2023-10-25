﻿using BusinessObject.DTOs.Request.CreateRequests;
using BusinessObject.Models;
using Repository.Interfaces;

namespace IDBMS_API.Services
{
    public class AdminService
    {
        private readonly IAdminRepository _repository;
        public AdminService(IAdminRepository repository)
        {
            _repository = repository;
        }   
        public IEnumerable<Admin> GetAll()
        {
            return _repository.GetAll();
        }
        public Admin? Get(Guid id)
        {
            return _repository.GetById(id);
        }
        public Admin? CreateAdmin(AdminRequest request)
        {
            var admin = new Admin
            {
                Name = request.Name,
                Username = request.Username,
                Email = request.Email,
                PasswordHash = request.PasswordHash,
                PasswordSalt = request.PasswordSalt,
                AuthenticationCode = request.AuthenticationCode,
                IsDeleted = request.IsDeleted,
                CreatorId = request.CreatorId,
            };
            var adminCreated = _repository.Save(admin);
            return adminCreated;
        }
        public void UpdateAdmin(AdminRequest request)
        {
            var admin = new Admin
            {
                Name = request.Name,
                Username = request.Username,
                Email = request.Email,
                PasswordHash = request.PasswordHash,
                PasswordSalt = request.PasswordSalt,
                AuthenticationCode = request.AuthenticationCode,
                IsDeleted = request.IsDeleted,
                CreatorId = request.CreatorId,
            };
            _repository.Update(admin);
        }
        public void DeleteAdmin(Guid id)
        {
            _repository.DeleteById(id);
        }
    }
}