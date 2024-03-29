﻿using API.Supporters.JwtAuthSupport;
using Azure;
using Azure.Core;
using BusinessObject.Enums;
using BusinessObject.Models;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using IDBMS_API.DTOs.Request;
using IDBMS_API.DTOs.Response;
using IDBMS_API.Services;
using IDBMS_API.Services.PaginationService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repository.Interfaces;
using System.Threading.Tasks;

namespace IDBMS_API.Controllers.IDBMSControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectParticipationsController : ODataController
    {
        private readonly ProjectParticipationService _service;
        private readonly PaginationService<ProjectParticipation> _paginationService;

        public ProjectParticipationsController(ProjectParticipationService service, PaginationService<ProjectParticipation> paginationService)
        {
            _service = service;
            _paginationService = paginationService;
        }

        [HttpGet("")]
        [Authorize(Policy = "User")]
        public IActionResult GetParticpationInProjectByUserId(Guid userId, Guid projectId)
        {
            try
            {
                var data = _service.GetParticpationInProjectByUserId(userId, projectId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = data
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }

        [HttpGet("decision-makers")]
        [Authorize(Policy = "Participation")]
        public IActionResult GetProjectDecisionMakersByProjectId(Guid projectId)
        {
            try
            {
                var owner = _service.GetProjectOwnerByProjectId(projectId);
                var pm = _service.GetProjectManagerByProjectId(projectId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = new
                    {
                        ProjectOwner = owner,
                        ProjectManager = pm,
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }

        [HttpGet("user-view")]
        [Authorize(Policy = "Participation")]
        public IActionResult GetProjectParticipationInProjectByUserView(Guid projectId, string? name, int? pageSize, int? pageNo)
        {
            try
            {
                var customerViewers = _service.GetCustomerViewersByProjectId(projectId, name);
                var pm = _service.GetProjectManagerByProjectId(projectId);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = new
                    {
                        CustomerViewers = _paginationService.PaginateList(customerViewers, pageSize, pageNo),
                        ProjectManager = pm,
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }

        [HttpGet("user/{id}")]
        [Authorize(Policy = "User")]
        public IActionResult GetParticipationsByUserId(Guid id, ParticipationRole? role, int? pageSize, int? pageNo, ProjectStatus? projectStatus, string? projectName)
        {
            try
            {
                var list = _service.GetByUserId(id, role, projectStatus, projectName);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(list, pageSize, pageNo)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }

        [HttpGet("project/{id}")]
        [Authorize(Policy = "ProjectManager")]
        public IActionResult GetParticipationsByProjectId(Guid projectId, Guid id, ParticipationRole? role, string? name, int? pageSize, int? pageNo)
        {
            try
            {
                var list = _service.GetProjectMemberByProjectId(id, role, name);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = new
                    {
                        PaginatedList = _paginationService.PaginateList(list, pageSize, pageNo),
                        ProductOwner = _service.GetProjectOwnerByProjectId(id),
                        ProjectManager = _service.GetProjectManagerByProjectId(id),
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }



        [HttpGet("project/{id}/users")]
        [Authorize(Policy = "Participation")]
        public IActionResult GetUsersByParticipationInProject(Guid projectId, Guid id)
        {
            try
            {
                var list = _service.GetUsersByParticipationInProject(id);

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = list
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }

        [HttpPost]
        [Authorize(Policy = "ProjectManager")]
        public IActionResult CreateParticipation(Guid projectId, [FromBody] ProjectParticipationRequest request)
        {
            try
            {
                var result = _service.CreateParticipation(request);
                var response = new ResponseMessage()
                {
                    Message = "Create successfully!",
                    Data = result
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }
        [HttpPost("addviewer")]
        [Authorize(Policy = "owner")]
        public IActionResult AddViewer(Guid projectId, [FromBody] string email)
        {
            try
            {
                var result = _service.AddViewer(projectId,email);
                var response = new ResponseMessage()
                {
                    Message = "Create successfully!",
                    Data = result
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }

        [HttpPost("employees")]
        [Authorize(Policy = "ProjectManager")]
        public IActionResult CreateParticipationsByRole(Guid projectId, [FromBody] CreateParticipationListRequest request)
        {
            try
            {
                var result = _service.CreateParticipationsByRole(request);
                var response = new ResponseMessage()
                {
                    Message = "Create successfully!",
                    Data = result,
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "ProjectManager")]
        public IActionResult UpdateParticipation(Guid projectId, Guid id, [FromBody] ProjectParticipationRequest request)
        {
            try
            {
                _service.UpdateParticipation(id, request);
                var response = new ResponseMessage()
                {
                    Message = "Update successfully!",
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "ProjectManager")]
        public IActionResult DeleteParticipation(Guid projectId, Guid id)
        {
            try
            {
                _service.DeleteParticipation(id);
                var response = new ResponseMessage()
                {
                    Message = "Delete successfully!",
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }
    }
}
