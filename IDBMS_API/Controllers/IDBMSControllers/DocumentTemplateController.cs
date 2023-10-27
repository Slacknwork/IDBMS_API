﻿using BusinessObject.DTOs.Request;
using IDBMS_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace IDBMS_API.Controllers.IDBMSControllers
{
    public class DocumentTemplateController : ODataController
    {
        private readonly DocumentTemplateService _service;

        public DocumentTemplateController(DocumentTemplateService service)
        {
            _service = service;
        }

        [EnableQuery]
        [HttpGet]
        public IActionResult GetDocumentTemplates()
        {
            return Ok(_service.GetAll());
        }

        [HttpPost]
        public IActionResult CreateDocumentTemplate([FromBody] ProjectDocumentTemplateRequest request)
        {
            try
            {
                _service.CreateDocumentTemplate(request);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDocumentTemplate(int id, [FromBody] ProjectDocumentTemplateRequest request)
        {
            try
            {
                _service.UpdateDocumentTemplate(id, request);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDocumentTemplate(int id)
        {
            try
            {
                _service.DeleteDocumentTemplate(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Ok();
        }
    }

}
