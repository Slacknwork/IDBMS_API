﻿using Azure.Core;
using IDBMS_API.DTOs.Request;
using BusinessObject.Models;
using DocumentFormat.OpenXml.Office2016.Excel;
using Repository.Interfaces;
using DocumentFormat.OpenXml.Office2010.Excel;
using BusinessObject.Enums;
using UnidecodeSharpFork;
using DocumentFormat.OpenXml.Wordprocessing;
using Repository.Implements;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using IDBMS_API.DTOs.Response;
using IDBMS_API.Supporters.TimeHelper;

namespace IDBMS_API.Services
{
    public class PaymentStageService
    {
        private readonly IPaymentStageRepository _stageRepo;
        private readonly IProjectRepository _projectRepo;
        private readonly IProjectDesignRepository _projectDesignRepo;
        private readonly IPaymentStageDesignRepository _stageDesignRepo;
        private readonly IProjectTaskRepository _taskRepo;
        private readonly IFloorRepository _floorRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly IRoomTypeRepository _roomTypeRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly ITaskDesignRepository _taskDesignRepo;
        private readonly ITaskCategoryRepository _taskCategoryRepo;

        public PaymentStageService(
            IPaymentStageRepository stageRepo,
            IProjectRepository projectRepo,
            IProjectDesignRepository projectDesignRepo,
            IPaymentStageDesignRepository stageDesignRepo,
            IProjectTaskRepository taskRepo,
            IFloorRepository floorRepo,
            IRoomRepository roomRepo,
            IRoomTypeRepository roomTypeRepo,
            ITransactionRepository transactionRepo,
            ITaskDesignRepository taskDesignRepo,
            ITaskCategoryRepository taskCategoryRepo)
        {
            _stageRepo = stageRepo;
            _projectRepo = projectRepo;
            _projectDesignRepo = projectDesignRepo;
            _stageDesignRepo = stageDesignRepo;
            _taskRepo = taskRepo;
            _floorRepo = floorRepo;
            _roomRepo = roomRepo;
            _roomTypeRepo = roomTypeRepo;
            _transactionRepo = transactionRepo;
            _taskDesignRepo = taskDesignRepo;
            _taskCategoryRepo = taskCategoryRepo;
        }

        public IEnumerable<PaymentStage> Filter(IEnumerable<PaymentStage> list,
           StageStatus? status, string? name)
        {
            IEnumerable<PaymentStage> filteredList = list;

            if (status != null)
            {
                filteredList = filteredList.Where(item => item.Status == status);
            }

            if (name != null)
            {
                filteredList = filteredList.Where(item => (item.Name != null && item.Name.Unidecode().IndexOf(name.Unidecode(), StringComparison.OrdinalIgnoreCase) >= 0));
            }

            return filteredList;
        }
        public IEnumerable<PaymentStage> GetAll(StageStatus? status, string? name)
        {
            var list = _stageRepo.GetAll();

            return Filter(list, status, name);
        }

        public IEnumerable<PaymentStage> GetOutOfDateStage()
        {
            
            var list = _stageRepo.GetAll().Where(s=>s.IsPrepaid!=true && s.EndTimePayment!=null && s.EndTimePayment.Value.Date <= TimeHelper.GetTime(DateTime.Now).Date);
            
            return list;
        }
        public IEnumerable<PaymentStage> GetAbout10ToExpireStage()
        {
            var list = _stageRepo.GetAll().Where(s=>s.IsPrepaid!=true && s.EndTimePayment!=null && s.EndTimePayment >= TimeHelper.GetTime(DateTime.Now)&& s.EndTimePayment.Value.AddDays(10) <= TimeHelper.GetTime(DateTime.Now));
            
            return list;
        }

        public PaymentStage? GetById(Guid id)
        {
            return _stageRepo.GetById(id) ?? throw new Exception("This payment stage id is not existed!");
        }

        public IEnumerable<PaymentStage> GetByProjectId(Guid projectId, StageStatus? status, string? name)
        {
            var list = _stageRepo.GetByProjectId(projectId);

            return Filter(list, status, name);
        }

        public IEnumerable<PaymentStageResponse> GetByProjectIdWithActionAllowed(Guid projectId, StageStatus? status, string? name)
        {
            var stageList = _stageRepo.GetByProjectId(projectId);
            var filteredList = Filter(stageList, status, name);

            var responseStages = stageList.Select(stage => new PaymentStageResponse { Stage = stage }).ToList();

            foreach (var response in responseStages)
            {
                var previousStage = _stageRepo.GetByStageNoByProjectId(response.Stage.StageNo - 1, projectId);

                bool isCurrentStageExceedDeadline = IsExceedPaymentDeadline(response.Stage.Id, response.Stage.EndTimePayment);
                bool isPreviousStageExceedDeadline = IsExceedPaymentDeadline(response.Stage.Id, response.Stage.EndTimePayment);

                if (response.Stage.Status == StageStatus.Unopen)
                {
                    //check if last stage or current stage is exceed deadline
                    if (isCurrentStageExceedDeadline || isPreviousStageExceedDeadline)
                    {
                        //suspend if first stage or last stage is not paid
                        if (previousStage == null ||  (previousStage != null && previousStage.Status == StageStatus.Done))
                        {
                            response.SuspendAllowed = true;
                        }
                    }
                    else
                    {
                        //check if current stage is first stage or last stage are done
                        if (response.Stage.StageNo == 1 || (previousStage != null && previousStage.Status == StageStatus.Done))
                        {
                            response.OpenAllowed = true;
                        }
                    }
                }

                if (response.Stage.Status == StageStatus.Ongoing)
                {
                    ProjectTaskService taskService = new(_taskRepo, _projectRepo, _stageRepo, _projectDesignRepo, _stageDesignRepo, _floorRepo, _roomRepo, _roomTypeRepo, _transactionRepo, _taskCategoryRepo, _taskDesignRepo);
                    var checkStageTaskDone = taskService.CheckFinishedTaskInStage(response.Stage.Id);

                    var nextStage = _stageRepo.GetByStageNoByProjectId(response.Stage.StageNo + 1, projectId);

                    if (checkStageTaskDone && ((nextStage != null) || (nextStage == null && response.Stage.IsContractAmountPaid == true && response.Stage.IsIncurredAmountPaid == true)))
                        response.CloseAllowed = true;
                }

                if (response.Stage.Status == StageStatus.Suspended)
                {
                    response.ReopenAllowed = true;
                }

            }

            return responseStages;
        }

        public PaymentStage? CreatePaymentStage(PaymentStageRequest request)
        {
            ProjectService projectService = new(_projectRepo, _roomRepo, _roomTypeRepo, _taskRepo, _stageRepo, _projectDesignRepo, _stageDesignRepo, _floorRepo, _transactionRepo, _taskDesignRepo, _taskCategoryRepo);
            var project = projectService.GetById(request.ProjectId);

            var ps = new PaymentStage
            {
                Id = Guid.NewGuid(),
                StageNo = 0,
                Name = request.Name,
                Description = request.Description,
                TotalContractPaid = (decimal)(request.PricePercentage / 100) * (decimal)project.EstimatedPrice,
                IsPrepaid = request.IsPrepaid,
                PricePercentage = request.PricePercentage,
                EndTimePayment = request.EndTimePayment,
                ProjectId = request.ProjectId,
                IsDeleted = false,
                Status = StageStatus.Unopen,
                EstimateBusinessDay = 0,
                IsWarrantyStage= request.IsWarrantyStage,
            };

            var psCreated = _stageRepo.Save(ps);

            UpdateStageNoByProjectId(request.ProjectId);

            if (psCreated != null && request.IsWarrantyStage)
            {
                UpdateWarrantyStage(psCreated.Id, psCreated.ProjectId);
            }

            if (psCreated != null)
            {
                UpdateEndTimePayment(psCreated.Id);
            }

            return psCreated;
        }
        public User GetOwner(Guid stageId)
        {
            var stage = _stageRepo.GetById(stageId);
            var project = _projectRepo.GetById(stage.ProjectId);
            return project.ProjectParticipations.FirstOrDefault(p => p.Role == ParticipationRole.ProductOwner).User;
        }
        public void CreatePaymentStagesByProjectDesign(Guid projectId)
        {
            ProjectService projectService = new(_projectRepo, _roomRepo, _roomTypeRepo, _taskRepo, _stageRepo, _projectDesignRepo, _stageDesignRepo, _floorRepo, _transactionRepo, _taskDesignRepo, _taskCategoryRepo);
            var project = projectService.GetById(projectId);

            ProjectDesignService pjDesignService = new ProjectDesignService(_projectDesignRepo);
            var selectedDesign = pjDesignService.GetProjectDesignMapped(project.EstimatedPrice.Value, project.Type);

            if (selectedDesign == null)
            {
                throw new Exception("No suitable project design found for the given budget range!");
            }

            PaymentStageDesignService pmDesignService = new (_stageDesignRepo);
            var pmDesigns = pmDesignService.GetByProjectDesignId(selectedDesign.Id, null);

            foreach (var stage in pmDesigns)
            {
                var ps = new PaymentStage
                {
                    Id = Guid.NewGuid(),
                    StageNo = stage.StageNo,
                    Name = stage.Name,
                    Description = stage.Description,
                    TotalContractPaid = (decimal)(stage.PricePercentage / 100) * (decimal)project.EstimatedPrice,
                    IsPrepaid = stage.IsPrepaid,
                    PricePercentage = stage.PricePercentage,
                    ProjectId = projectId,
                    IsDeleted = false,
                    IsWarrantyStage= stage.IsWarrantyStage,
                    Status = StageStatus.Unopen
                };

                var stageCreated = _stageRepo.Save(ps);

                if (stageCreated != null && stageCreated.IsWarrantyStage)
                {
                    UpdateWarrantyStage(stageCreated.Id, stageCreated.ProjectId);
                }

                if (stageCreated!= null)
                {
                    UpdateEndTimePayment(stageCreated.Id);
                }
            }
        }


        public void UpdatePaymentStage(Guid id, PaymentStageRequest request)
        {
            var ps = _stageRepo.GetById(id) ?? throw new Exception("This payment stage id is not existed!");

            ps.StageNo = 0;
            ps.Name = request.Name;
            ps.Description = request.Description;
            ps.IsPrepaid = request.IsPrepaid;
            ps.PricePercentage = request.PricePercentage;
            ps.ProjectId = request.ProjectId;
            ps.IsWarrantyStage = request.IsWarrantyStage;

            if (request.EndTimePayment == ps.EndTimePayment)
                UpdateEndTimePayment(id);
            else
                ps.EndTimePayment = request.EndTimePayment;

            ProjectService projectService = new (_projectRepo, _roomRepo, _roomTypeRepo, _taskRepo, _stageRepo, _projectDesignRepo, _stageDesignRepo, _floorRepo, _transactionRepo, _taskDesignRepo, _taskCategoryRepo);
            var project = projectService.GetById(request.ProjectId);

            ps.TotalContractPaid = (decimal)(request.PricePercentage / 100) * (decimal)project.EstimatedPrice;
            
            _stageRepo.Update(ps);

            if (ps.IsWarrantyStage)
            {
                UpdateWarrantyStage(ps.Id, ps.ProjectId);
            }

            UpdateStageNoByProjectId(ps.ProjectId);
        }

        public void UpdateEndTimePayment(Guid stageId)
        {
            var stage = GetById(stageId) ?? throw new Exception("This payment stage id is not existed!");

            stage.EndTimePayment = CalculateEndTimePayment(stage.StartedDate, stage.EndDate, stage.IsPrepaid);

            _stageRepo.Update(stage);
            UpdateProjectWarrantyEnd(stage.ProjectId);
        }

        private DateTime? CalculateEndTimePayment(DateTime? startDate, DateTime? endDate, bool isPrepaid)
        {
            if (startDate == null || endDate == null)
            {
                return null;
            }
            else
            {
                if (isPrepaid)
                {
                    return startDate.Value.AddDays(-10);
                }
                else
                {
                    return endDate.Value.AddDays(10);
                }
            }
        }

        public void UpdateStageNoByProjectId(Guid projectId)
        {
            var stageList = _stageRepo.GetByProjectId(projectId)
                                .OrderBy(s => s.EndTimePayment.HasValue)
                                    .ThenByDescending(s => s.EndTimePayment) 
                                .ToList();

            var nullEndTimePaymentList = stageList.Where(s => s.EndTimePayment == null).ToList();
            var nonNullEndTimePaymentList = stageList.Where(s => s.EndTimePayment != null).ToList();

            nullEndTimePaymentList.Reverse();

            var reversedStageList = nonNullEndTimePaymentList.Concat(nullEndTimePaymentList).ToList();

            int stageNumber = 1;

            foreach (var stage in reversedStageList)
            {
                stage.StageNo = stageNumber++;
                _stageRepo.Update(stage);
            }

            UpdateProjectWarrantyEnd(projectId);
        }

        public void UpdateStageTimeSpan(Guid stageId, DateTime? soonestStartDate, DateTime? latestEndDate)
        {
            var stage = _stageRepo.GetById(stageId) ?? throw new Exception("This payment stage id is not existed!");

            stage.StartedDate= soonestStartDate;
            stage.EndDate= latestEndDate;
            stage.EndTimePayment = CalculateEndTimePayment(stage.StartedDate, stage.EndDate, stage.IsPrepaid);

            _stageRepo.Update(stage);

            UpdateProjectWarrantyEnd(stage.ProjectId);
        }

        private bool IsExceedPaymentDeadline(Guid stageId, DateTime? endTime)
        {
            var stage = _stageRepo.GetById(stageId) ?? throw new Exception("This payment stage id is not existed!");

            //check if incurred amount in last stage is paid or not
            if (stage.StageNo != 1)
            {
                var previousStage = _stageRepo.GetByStageNoByProjectId(stage.StageNo - 1, stage.ProjectId);
                
                if (previousStage != null && previousStage.IsIncurredAmountPaid == false)
                {
                    return true;
                }
            }

            if (stage.IsContractAmountPaid == false)
            {
                if (endTime.HasValue)
                {
                    DateTime currentDate = TimeHelper.GetTime(DateTime.Now);

                    return currentDate > endTime;
                }
            }

            return false;
        }

        public void StartStage(Guid id)
        {
            var ps = _stageRepo.GetById(id) ?? throw new Exception("This payment stage id is not existed!");

            ps.StartedDate = TimeHelper.GetTime(DateTime.Now);
            ps.Status = StageStatus.Ongoing;

            _stageRepo.Update(ps);

/*            ProjectTaskService taskService = new (_taskRepo, _projectRepo, _stageRepo, _projectDesignRepo, _stageDesignRepo, _floorRepo, _roomRepo, _roomTypeRepo, _transactionRepo);
            taskService.StartTasksOfStage(id, ps.ProjectId);*/

            ProjectService projectService = new(_projectRepo, _roomRepo, _roomTypeRepo, _taskRepo, _stageRepo, _projectDesignRepo, _stageDesignRepo, _floorRepo, _transactionRepo, _taskDesignRepo, _taskCategoryRepo);
            var project = projectService.GetById(ps.ProjectId);

            if (project != null)
            {
                if (project.Status != ProjectStatus.Ongoing && project.Status != ProjectStatus.WarrantyPending)
                {
                    projectService.UpdateProjectStatus(project.Id, ProjectStatus.Ongoing);
                }

                if (project.Status != ProjectStatus.WarrantyPending && ps.IsWarrantyStage == true)
                {
                    projectService.UpdateProjectStatus(project.Id, ProjectStatus.WarrantyPending);
                }
            }
        }

        public void CloseStage(Guid id)
        {
            var ps = _stageRepo.GetById(id) ?? throw new Exception("This payment stage id is not existed!");

            ProjectTaskService taskService = new (_taskRepo, _projectRepo, _stageRepo, _projectDesignRepo, _stageDesignRepo, _floorRepo, _roomRepo, _roomTypeRepo, _transactionRepo, _taskCategoryRepo, _taskDesignRepo);
            var tasksOfStage = taskService.GetAllProjectTaskIdByFilter(ps.ProjectId, null, id, ProjectTaskStatus.Ongoing, null, null, false, true, null);

            if (tasksOfStage.Count() > 0)
                throw new Exception("Some Tasks in Stage is not done!");

            ps.EndDate = TimeHelper.GetTime(DateTime.Now);
            ps.Status = StageStatus.Done;

            _stageRepo.Update(ps);

            ProjectService projectService = new(_projectRepo, _roomRepo, _roomTypeRepo, _taskRepo, _stageRepo, _projectDesignRepo, _stageDesignRepo, _floorRepo, _transactionRepo, _taskDesignRepo, _taskCategoryRepo);
            var project = projectService.GetById(ps.ProjectId);

            if (project != null)
            {
                //check if next stage is exist or not
                var nextStage = _stageRepo.GetByStageNoByProjectId(ps.StageNo + 1, project.Id);

                if (nextStage == null)
                {
                    projectService.UpdateProjectStatus(project.Id, ProjectStatus.Done);
                }
                else
                {
                    if (nextStage.IsWarrantyStage == true)
                    {
                        projectService.UpdateProjectStatus(project.Id, ProjectStatus.WarrantyPending);
                    }
                    else
                    {
                        projectService.UpdateProjectStatus(project.Id, ProjectStatus.PendingDeposit);
                    }
                }
            }
        }

        public void SuspendStage(Guid id)
        {
            var ps = _stageRepo.GetById(id) ?? throw new Exception("This payment stage id is not existed!");

            ps.Status = StageStatus.Suspended;

            _stageRepo.Update(ps);

            ProjectService projectService = new(_projectRepo, _roomRepo, _roomTypeRepo, _taskRepo, _stageRepo, _projectDesignRepo, _stageDesignRepo, _floorRepo, _transactionRepo, _taskDesignRepo, _taskCategoryRepo);
            var project = projectService.GetById(ps.ProjectId);

            if (project != null)
            {
                projectService.UpdateProjectStatus(project.Id, ProjectStatus.Suspended);
            }
        }        
        
        public void ReopenStage(Guid id, decimal penaltyFee)
        {
            var ps = _stageRepo.GetById(id) ?? throw new Exception("This payment stage id is not existed!");

            ps.Status = StageStatus.Unopen;

            _stageRepo.Update(ps);

            UpdateStagePenaltyFee(id, penaltyFee);

            ProjectService projectService = new(_projectRepo, _roomRepo, _roomTypeRepo, _taskRepo, _stageRepo, _projectDesignRepo, _stageDesignRepo, _floorRepo, _transactionRepo, _taskDesignRepo, _taskCategoryRepo);
            var project = projectService.GetById(ps.ProjectId);

            if (project != null)
            {
                projectService.UpdateProjectStatus(project.Id, ProjectStatus.PendingDeposit);
            }
        }

        public void UpdateStageEstimateBusinessDay(Guid id, int estimateBusinessDay)
        {
            var ps = _stageRepo.GetById(id) ?? throw new Exception("This payment stage id is not existed!");

            ps.EstimateBusinessDay = estimateBusinessDay;

            _stageRepo.Update(ps);
        }

        public void UpdateStagePenaltyFee(Guid stageId, decimal penaltyFee)
        {
            var ps = _stageRepo.GetById(stageId) ?? throw new Exception("This payment stage id is not existed!");

            ps.PenaltyFee = penaltyFee;

            _stageRepo.Update(ps);

            UpdateProjectPenaltyFeeByStages(ps.ProjectId);
        }

        public void UpdateProjectPenaltyFeeByStages(Guid projectId)
        {
            var stageList = _stageRepo.GetByProjectId(projectId);

            decimal totalPenaltyFee = stageList.Sum(stage => stage.PenaltyFee ?? 0);

            ProjectService projectService = new(_projectRepo, _roomRepo, _roomTypeRepo, _taskRepo, _stageRepo, _projectDesignRepo, _stageDesignRepo, _floorRepo, _transactionRepo, _taskDesignRepo, _taskCategoryRepo);
            projectService.UpdateProjectTotalPenaltyFee(projectId, totalPenaltyFee);

            TransactionService transactionService = new TransactionService(_transactionRepo, _stageRepo, _projectRepo, _projectDesignRepo, _stageDesignRepo, _taskRepo, _floorRepo, _roomRepo, _roomTypeRepo, _taskDesignRepo, _taskCategoryRepo);
            transactionService.UpdateTotalPaidByProjectId(projectId);
        }

        public void UpdateStagesTotalContractPaid(Guid projectId, decimal estimatePrice)
        {
            var stageList = _stageRepo.GetByProjectId(projectId);

            foreach (var stage in stageList)
            {
                stage.TotalContractPaid = (decimal)(stage.PricePercentage / 100) * (decimal)estimatePrice;

                _stageRepo.Update(stage);
            }
        }

        public void UpdateStageTotalIncurredPaid(Guid stageId, decimal totalIncurredPaid)
        {
            var stage = _stageRepo.GetById(stageId) ?? throw new Exception("This payment stage id is not existed!");

            stage.TotalIncurredPaid = totalIncurredPaid == 0 ? null : totalIncurredPaid;

            _stageRepo.Update(stage);

            TransactionService transactionService = new TransactionService(_transactionRepo, _stageRepo, _projectRepo, _projectDesignRepo, _stageDesignRepo, _taskRepo, _floorRepo, _roomRepo, _roomTypeRepo, _taskDesignRepo, _taskCategoryRepo);
            transactionService.UpdateTotalPaidByProjectId(stage.ProjectId);
        }

        public void UpdateStagePaid(Guid projectId, decimal totalPaid)
        {
            var listStages = _stageRepo.GetByProjectId(projectId)
                                        .OrderBy(s => s.StageNo)
                                        .ToList();

            decimal totalNegativeIncurred = listStages
                        .Where(stage => stage.TotalIncurredPaid != null && stage.TotalIncurredPaid < 0)
                        .Sum(stage => Math.Abs(stage.TotalIncurredPaid.Value));

            decimal remainingAmount = totalPaid + totalNegativeIncurred;

            bool stopCheck = false;

            foreach (var stage in listStages)
            {
                stage.IsContractAmountPaid = false;
                stage.IsIncurredAmountPaid = false;

                if (stopCheck == false)
                {
                    if (remainingAmount >= stage.TotalContractPaid)
                    {
                        stage.IsContractAmountPaid = true;
                        remainingAmount -= stage.TotalContractPaid;
                    }
                    else
                    {
                        _stageRepo.Update(stage);
                        stopCheck= true;
                        continue;
                    }

                    if ((stage.TotalIncurredPaid == null && stage.PenaltyFee == null) || stage.TotalIncurredPaid < 0)
                        stage.IsIncurredAmountPaid = true;
                    else
                    {
                        var incurredTotal = stage.TotalIncurredPaid == null ? 0 : stage.TotalIncurredPaid;
                        var penaltyFeeTotal = stage.PenaltyFee == null ? 0 : stage.PenaltyFee;

                        if (remainingAmount >= incurredTotal + penaltyFeeTotal)
                        {
                            stage.IsIncurredAmountPaid = true;
                            remainingAmount -= incurredTotal.Value;
                            remainingAmount -= penaltyFeeTotal.Value;
                        }
                    }

                    _stageRepo.Update(stage);
                }
                else
                    _stageRepo.Update(stage);

            }
        }

        public void UpdateWarrantyStage(Guid warrantyStageId, Guid projectId)
        {
            var stageList = _stageRepo.GetByProjectId(projectId);

            foreach (var stage in stageList)
            {
                if (stage.Id == warrantyStageId)
                {
                    stage.IsWarrantyStage = true;
                }
                else
                {
                    stage.IsWarrantyStage = false;
                }

                _stageRepo.Update(stage);
            }

            UpdateProjectWarrantyEnd(projectId);
        }

        public void UpdateProjectWarrantyEnd(Guid projectId)
        {
            var stageList = _stageRepo.GetByProjectId(projectId);

            var lastStage = stageList
                    .OrderByDescending(stage => stage.StageNo)
                    .FirstOrDefault();

            if (lastStage != null)
            {
                ProjectService projectService = new(_projectRepo, _roomRepo, _roomTypeRepo, _taskRepo, _stageRepo, _projectDesignRepo, _stageDesignRepo, _floorRepo, _transactionRepo, _taskDesignRepo, _taskCategoryRepo);
                projectService.UpdateProjectWarrantyPeriodEndTime(projectId, lastStage.EndTimePayment ?? null);
            }
        }

        public void DeletePaymentStage(Guid id)
        {
            var ps = _stageRepo.GetById(id) ?? throw new Exception("This payment stage id is not existed!");

            ps.IsDeleted = true;

            _stageRepo.Update(ps);

            ProjectTaskService taskService = new (_taskRepo, _projectRepo, _stageRepo, _projectDesignRepo, _stageDesignRepo, _floorRepo, _roomRepo, _roomTypeRepo, _transactionRepo, _taskCategoryRepo, _taskDesignRepo);
            taskService.UpdateTasksInDeletedStage(id, ps.ProjectId);

            UpdateProjectPenaltyFeeByStages(ps.ProjectId);
            UpdateProjectWarrantyEnd(ps.ProjectId);
        }
    }
}
