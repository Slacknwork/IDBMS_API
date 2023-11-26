﻿using BusinessObject.DTOs.Request;
using BusinessObject.Enums;
using BusinessObject.Models;
using Repository.Interfaces;

namespace IDBMS_API.Services
{
    public class ProjectTaskService
    {
        private readonly IProjectTaskRepository _repository;
        public ProjectTaskService(IProjectTaskRepository repository)
        {
            _repository = repository;
        }
        public IEnumerable<ProjectTask> GetAll()
        {
            return _repository.GetAll();
        }
        public ProjectTask? GetById(Guid id)
        {
            return _repository.GetById(id) ?? throw new Exception("This object is not existed!");
        }
        public IEnumerable<ProjectTask?> GetByProjectId(Guid id)
        {
            return _repository.GetByProjectId(id) ?? throw new Exception("This object is not existed!");
        }
        public IEnumerable<ProjectTask?> GetByPaymentStageId(Guid id)
        {
            return _repository.GetByPaymentStageId(id) ?? throw new Exception("This object is not existed!");
        }
        public ProjectTask? CreateProjectTask(ProjectTaskRequest request)
        {
            var ct = new ProjectTask
            {
                Id = Guid.NewGuid(),
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                Percentage = request.Percentage,
                CalculationUnit = request.CalculationUnit,
                PricePerUnit = request.PricePerUnit,
                UnitInContract = request.UnitInContract,
                UnitUsed = request.UnitUsed,
                IsIncurred = request.IsIncurred,
                StartedDate = DateTime.Now,
                EndDate = request.EndDate,
                NoDate = request.NoDate,
                CreatedDate = request.CreatedDate,
                ProjectId = request.ProjectId,
                PaymentStageId = request.PaymentStageId,
                InteriorItemId = request.InteriorItemId,
                RoomId = request.RoomId,
                Status = request.Status,
            };
            var ctCreated = _repository.Save(ct);
            return ctCreated;
        }
        public void UpdateProjectTask(Guid id, ProjectTaskRequest request)
        {
            var ct = _repository.GetById(id) ?? throw new Exception("This object is not existed!");

            ct.Code = request.Code;
            ct.Name = request.Name;
            ct.Description = request.Description;
            ct.Percentage = request.Percentage;
            ct.CalculationUnit = request.CalculationUnit;
            ct.PricePerUnit = request.PricePerUnit;
            ct.UnitInContract = request.UnitInContract;
            ct.UnitUsed = request.UnitUsed;
            ct.IsIncurred = request.IsIncurred;
            ct.UpdatedDate= request.UpdatedDate;
            ct.EndDate = request.EndDate;
            ct.NoDate = request.NoDate;
            ct.ProjectId = request.ProjectId;
            ct.PaymentStageId = request.PaymentStageId;
            ct.InteriorItemId = request.InteriorItemId;
            ct.RoomId = request.RoomId;
            ct.Status = request.Status;

            _repository.Update(ct);
        }
        public void UpdateProjectTaskStatus(Guid id, ProjectTaskStatus status)
        {
            var ct = _repository.GetById(id) ?? throw new Exception("This object is not existed!");

            ct.Status = status;

            _repository.Update(ct);
        }
    }
}