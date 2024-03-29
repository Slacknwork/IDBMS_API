﻿using BLL.Services;
using BusinessObject.Enums;
using BusinessObject.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using IDBMS_API.DTOs.Request;
using IDBMS_API.DTOs.Request.AdvertisementRequest;
using IDBMS_API.Supporters.TimeHelper;
using Microsoft.AspNetCore.Mvc;
using Repository.Interfaces;
using System.Runtime.Intrinsics.Arm;
using UnidecodeSharpFork;
using static System.Net.Mime.MediaTypeNames;

namespace IDBMS_API.Services
{
    public class AdvertisementService
    {
        private readonly IProjectRepository _projectRepo;
        private readonly IProjectDocumentRepository _documentRepo;

        public AdvertisementService(IProjectRepository projectRepo, IProjectDocumentRepository documentRepo)
        {
            _projectRepo = projectRepo;
            _documentRepo = documentRepo;
        }

        private IEnumerable<Project> FilterProject(IEnumerable<Project> list,
                int? categoryId, ProjectType? type, AdvertisementStatus? status, string? name)
        {
            IEnumerable<Project> filteredList = list;

            if (categoryId != null)
            {
                filteredList = filteredList.Where(item => item.ProjectCategoryId == categoryId);
            }

            if (type != null)
            {
                filteredList = filteredList.Where(item => item.Type == type);
            }

            if (status != null)
            {
                filteredList = filteredList.Where(item => item.AdvertisementStatus == status);
            }

            if (name != null)
            {
                filteredList = filteredList.Where(item => (item.Name != null && item.Name.Unidecode().IndexOf(name.Unidecode(), StringComparison.OrdinalIgnoreCase) >= 0));
            }

            return filteredList;
        }

        private IEnumerable<ProjectDocument> FilterImage(IEnumerable<ProjectDocument> list,
        bool? isPublicAdvertisement)
        {
            IEnumerable<ProjectDocument> filteredList = list;

            if (isPublicAdvertisement != null)
            {
                filteredList = filteredList.Where(item => item.IsPublicAdvertisement == isPublicAdvertisement);
            }

            return filteredList;
        }

        public IEnumerable<Project> GetPublicProjects(int? categoryId, ProjectType? type, AdvertisementStatus? status, string? name)
        {
            var list = _projectRepo.GetPublicAdvertisementProjects();

            return FilterProject(list, categoryId, type, status, name);
        }

        public IEnumerable<Project> GetAllProjects(int? categoryId, ProjectType? type, AdvertisementStatus? status, string? name)
        {
            var list = _projectRepo.GetAdvertisementAllowedProjects();

            return FilterProject(list, categoryId, type, status, name);
        }

        public IEnumerable<ProjectDocument> GetImagesByProjectId(Guid projectId, bool? isPublicAdvertisement)
        {
            var list = _documentRepo.GetByProjectId(projectId).Where(d => d.Category == ProjectDocumentCategory.CompletionImage);

            return FilterImage(list, isPublicAdvertisement);
        }
        public Project GetAdProjectById(Guid id)
        {
            var adProject =  _projectRepo.GetById(id) ?? throw new Exception("This project id is not existed!");

            adProject.ProjectDocuments = GetImagesByProjectId(id, true).ToList();

            return adProject;
        }

        public Project? CreateAdvertisementProject(AdvertisementProjectRequest request)
        {
            var newAdProject = new Project
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = "Dự án quảng cáo",
                Type= request.Type,
                ProjectCategoryId= request.ProjectCategoryId,
                CreatedDate = TimeHelper.GetTime(DateTime.Now),
                CreatedAdminUsername= request.CreatedAdminUsername,
                CreatedByAdminId= request.CreatedByAdminId,
                EstimatedPrice= request.EstimatedPrice,
                FinalPrice = request.FinalPrice,
                AmountPaid = request.FinalPrice,
                Area = request.Area,
                Language= request.Language,
                AdvertisementStatus = AdvertisementStatus.Allowed,
                Status = ProjectStatus.Done,
                EstimateBusinessDay = request.EstimateBusinessDay,
            };

            var createdProject = _projectRepo.Save(newAdProject);

            return createdProject;
        }

        public void UpdateAdvertisementProject(Guid projectId, AdvertisementProjectRequest request)
        {
            var adsProject = _projectRepo.GetById(projectId) ?? throw new Exception("This project id is not existed!");

            adsProject.Name = request.Name;
            adsProject.Type = request.Type;
            adsProject.ProjectCategoryId = request.ProjectCategoryId;
            adsProject.EstimatedPrice = request.EstimatedPrice;
            adsProject.FinalPrice = request.FinalPrice;
            adsProject.AmountPaid = request.FinalPrice;
            adsProject.Area = request.Area;
            adsProject.Language = request.Language;
            adsProject.EstimateBusinessDay = request.EstimateBusinessDay;
            adsProject.UpdatedDate = TimeHelper.GetTime(DateTime.Now);

            _projectRepo.Update(adsProject);
        }

        public async Task CreateCompletionImage(AdvertisementImageRequest request)
        {
            if (request.ImageList.Any())
            {
                foreach (var imageRequest in request.ImageList)
                {
                    var image = new ProjectDocument
                    {
                        Id = Guid.NewGuid(),
                        Name = "Advertisement Image",
                        CreatedDate = TimeHelper.GetTime(DateTime.Now),
                        Category = ProjectDocumentCategory.CompletionImage,
                        ProjectId = request.ProjectId,
                        IsPublicAdvertisement = request.IsPublicAdvertisement,
                        IsDeleted = false,
                    };

                    FirebaseService s = new FirebaseService();
                    string link = await s.UploadDocument(imageRequest, request.ProjectId);

                    image.Url = link;

                    _documentRepo.Save(image);
                }
            }
        }

        public void DeleteCompletionImage(Guid documentId)
        {
            var pd = _documentRepo.GetById(documentId) ?? throw new Exception("This project document id is not existed!");

            pd.IsDeleted = true;

            _documentRepo.Update(pd);
        }

        public void UpdatePublicImage(Guid documentId, bool isPublicAdvertisement)
        {
            var pd = _documentRepo.GetById(documentId) ?? throw new Exception("This project document id is not existed!");

            pd.IsPublicAdvertisement = isPublicAdvertisement;

            _documentRepo.Update(pd);
        }

        public async Task UpdateAdProjectDescription(Guid projectId, AdvertisementDescriptionRequest request)
        {
            var p = _projectRepo.GetById(projectId) ?? throw new Exception("This project id is not existed!");

            p.AdvertisementDescription = request.AdvertisementDescription;
            p.EnglishAdvertisementDescription = request.EnglishAdvertisementDescription;
            p.UpdatedDate = TimeHelper.GetTime(DateTime.Now);

            if (request.RepresentImage != null)
            {
                FirebaseService s = new FirebaseService();
                string link = await s.UploadDocument(request.RepresentImage, projectId);

                p.RepresentImageUrl = link;
            }

            _projectRepo.Update(p);
        }

        public void UpdateProjectAdvertisementStatus(Guid projectId, AdvertisementStatus status)
        {
            var project = _projectRepo.GetById(projectId) ?? throw new Exception("Not existed");

            project.AdvertisementStatus = status;
            project.UpdatedDate= TimeHelper.GetTime(DateTime.Now);

            _projectRepo.Update(project);
        }
    }
}
