﻿using BLL.Services;
using BusinessObject.Models;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using DocumentFormat.OpenXml.Packaging;
using IDBMS_API.Constants;
using IDBMS_API.DTOs.Request;
using IDBMS_API.Supporters.File;
using IDBMS_API.Supporters.Utils;
using Repository.Implements;

namespace IDBMS_API.Services.ExcelService
{
    public class ExcelService
    {
        byte[] _dataSample;
        byte[] _file;
        FirebaseService firebaseService;
        public ExcelService()
        {
            firebaseService = new FirebaseService();
        }
        public async Task<byte[]?> GenNewExcel(Guid request)
        {
            _dataSample = await firebaseService.DownloadFile("TemplateExcel.xlsx", null, "Excel", true);
            _file = ExcelSupporter.GenExcelFileBytes(_dataSample,request);
            return _file;
        }
    }
}
