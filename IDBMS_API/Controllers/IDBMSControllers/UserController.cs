﻿using BusinessObject.DTOs.Request;
using IDBMS_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace IDBMS_API.Controllers.IDBMSControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ODataController
    {
        [EnableQuery]
        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok();
        }
    }
}
