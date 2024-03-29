﻿using API.Supporters.JwtAuthSupport;
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
    public class NotificationsController : ODataController
    {
        private readonly NotificationService _service;
        private readonly PaginationService<Notification> _paginationService;

        public NotificationsController(NotificationService service, PaginationService<Notification> paginationService)
        {
            _service = service;
            _paginationService = paginationService;
        }

        [EnableQuery]
        [HttpGet]
        [Authorize(Policy = "User")]
        public IActionResult GetNotifications(NotificationCategory? category, int? pageSize, int? pageNo)
        {
            try
            {
                var list = _service.GetAll(category);

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
        [EnableQuery]
        [HttpGet("user/{id}")]
        [Authorize(Policy = "User")]
        public IActionResult GetByUserId(Guid id, NotificationCategory? category, int? pageSize, int? pageNo)
        {
            try
            {
                var list = _service.GetByUserId(id, category);

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

        [EnableQuery]
        [HttpGet("{id}")]
        [Authorize(Policy = "User")]
        public IActionResult GetNotificationById(Guid id)
        {
            try
            {
                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _service.GetById(id),
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
        [Authorize(Policy = "")]
        public IActionResult CreateNotificationsForAllCustomers([FromBody] NotificationRequest request)
        {
            try
            {
                _service.CreateNotificatonsForAllCustomers(request);

                var response = new ResponseMessage()
                {
                    Message = "Create successfully!"
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

        [HttpPost("projects/{id}")]
        [Authorize(Policy = "ProjectManager")]
        public IActionResult CreateNotificationForProject(Guid projectId, Guid id, [FromBody] NotificationRequest request)
        {
            try
            {
                _service.CreateNotificatonForProject(id, request);
                var response = new ResponseMessage()
                {
                    Message = "Create successfully!"
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
        [HttpPut("{id}/is-seen")]
        [Authorize(Policy = "User")]
        public IActionResult UpdateIsSeen(Guid id)
        {
            try
            {
                _service.UpdateIsSeenById(id);
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

        [HttpPut("is-seen/user/{id}")]
        [Authorize(Policy = "User")]
        public IActionResult UpdateIsSeenByUser(Guid id)
        {
            try
            {
                _service.UpdateIsSeenByUserId(id);
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
    }

}
