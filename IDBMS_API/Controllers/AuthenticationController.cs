﻿
using API.Services;
using API.Supporters.JwtAuthSupport;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using IDBMS_API.Supporters.Utils;
using IDBMS_API.Services;
using Azure;
using DocumentFormat.OpenXml.Spreadsheet;
using Azure.Core;
using DocumentFormat.OpenXml.Office2016.Excel;
using IDBMS_API.DTOs.Request.AccountRequest;
using IDBMS_API.DTOs.Response;
using IDBMS_API.Supporters.EmailSupporter;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationsController : ControllerBase
    {
        private readonly UserService userService;
        private readonly AdminService adminService;
        private readonly AuthenticationCodeService authenticationCodeService;
        private readonly IConfiguration configuration;

        public AuthenticationsController(UserService userService, AuthenticationCodeService authenticationCodeService,AdminService adminService )
        {
            this.userService = userService;
            this.authenticationCodeService = authenticationCodeService;
            this.adminService = adminService;
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            try
            {
                var (token, user) = userService.Login(request.Email, request.Password);
                var response = new ResponseMessage();
                if (token == null)
                {
                    response.Message = "Incorrect email or password!";
                    return BadRequest(response);
                }

                response.Message = "Login successfully!";
                response.Data = new
                {
                    Token = token,
                    user.Name,
                    user.Id,
                    Role = user.Role.ToString(),
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

        [HttpPost("loginByGoogle")]
        public IActionResult LoginByGoogle(LoginByGoogleRequest request)
        {
            try
            {
                var (token, user) = userService.LoginByGoogle(request);
                var response = new ResponseMessage();
                if (token == null)
                {
                    response.Message = "Incorrect email or password!";
                    return BadRequest(response);
                }

                response.Message = "Login successfully!";
                response.Data = new
                {
                    Token = token,
                    user.Name,
                    user.Id,
                    Role = user.Role.ToString(),
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

        [HttpPost("admin/login")]
        public IActionResult LoginAdmin(AdminLoginRequest request)
        {
            try
            {
                var (token, user) = adminService.Login(request.Username, request.Password);
                var response = new ResponseMessage();
                if (token == null)
                {
                    response.Message = "Incorrect username or password!";
                    return BadRequest(response);
                }

                response.Message = "Login successfully! please check your email to continue.";
                response.Data = new
                {
                    user.Username,
                    user.Name,
                    user.Id,
                    Role = "Admin",
                };
                var code = authenticationCodeService.CreateCode(user.Email);
                if (code == null) return BadRequest();
                string link = configuration["Server:Frontend"] + "/Authentication/adminConfirmverify?code=" + code + "&email=" + user.Email;
                EmailSupporter.SendVerifyEnglishEmail(user.Email, link);
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
        [HttpPut("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            try
            {
                var response = new ResponseMessage();
                var user = (User?)HttpContext.Items["User"];
                if (user != null)
                {
                    //userService.Logout(user);
                    response.Message = "Logout successfully!" ;
                    return Ok(response);
                }
                response.Message = "Cannot logout, user not existed";
                return BadRequest(response);
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
        [HttpPost("register")]
        public IActionResult Register(CreateUserRequest request)
        {
            try
            {
                var user = userService.CreateUser(request);
                if (user == null) return BadRequest("Email already exist!");
                var response = new ResponseMessage()
                {
                    Message = "Update successfully!",
                };
                return Login(new LoginRequest() { Email = request.Email, Password = request.Password });
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
        
        [HttpPut("password")]
        public IActionResult UpdateUserPassword(UpdatePasswordRequest request)
        {
            try
            {
                userService.UpdateUserPassword(request);
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

        [HttpPost("verify")]
        public IActionResult Verify(string email)
        {

            var code = authenticationCodeService.CreateCode(email);
            if (code == null) return BadRequest();
            string link = configuration["Server:Frontend"] + "/Authentication/confirmverify?code=" + code + "&email=" + email;
            EmailSupporter.SendVerifyEnglishEmail(email,link);
            return Ok();
        }
        [HttpPost("adminVerify")]
        public IActionResult AdminVerify(string email)
        {

            var code = authenticationCodeService.CreateCode(email);
            if (code == null) return BadRequest();
            string link = configuration["Server:Frontend"] + "/Authentication/adminConfirmverify?code=" + code + "&email=" + email;
            EmailSupporter.SendVerifyEnglishEmail(email,link);
            return Ok();
        }
        [HttpGet("confirmverify")]
        public IActionResult ConfirmVerify(string code,string email)
        {
            if (authenticationCodeService.Verify(code, email)) return Ok();
            return Unauthorized();

        }
        [HttpGet("adminConfirmverify")]
        public IActionResult AdminConfirmVerify(string code,string email)
        {
            string token = authenticationCodeService.AdminVerify(code, email);
            if (token!=null) return Ok(token);
            return Unauthorized();

        }
    }
}
