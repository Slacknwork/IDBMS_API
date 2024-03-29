﻿using API.Supporters.JwtAuthSupport;
using IDBMS_API.DTOs.Request;
using IDBMS_API.DTOs.Response;
using IDBMS_API.Services;
using IDBMS_API.Services.ExcelService;
using Microsoft.AspNetCore.Mvc;

namespace IDBMS_API.Controllers.IDBMSControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : Controller
    {
        ExcelService excelService;
        public ExcelController(ExcelService excelService)
        {
            this.excelService = excelService;
        }


        [HttpPost]
        //[Authorize(Policy = "")]
        public async Task<IActionResult> GenExcel(Guid projectId)
        {
            try
            {
                byte[] file = await excelService.GenNewExcel(projectId);
                //string fileName = "Contract-"+projectid.ToString()+".docx";
                string fileName = "TemplateExcel.xlsx";

                if (file == null)
                {
                    throw new Exception("File generated null!");
                }
                var response = new ResponseMessage()
                {
                    Message = "Success",
                    Data = File(file, "application/octet-stream", fileName)
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
