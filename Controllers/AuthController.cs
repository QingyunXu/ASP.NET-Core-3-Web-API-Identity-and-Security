using System;
using System.Threading.Tasks;
using Dotnet_Core_3_API.Dto.Auth;
using Dotnet_Core_3_API.Services;
using Dotnet_Core_3_API.Services.Auth;
using Dotnet_Core_3_API.Services.SMTP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Dotnet_Core_3_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly ISmtpService smtpService;
        private readonly IConfiguration configuration;
        public AuthController(IAuthService authService, ISmtpService smtpService, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.smtpService = smtpService;
            this.authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            ServiceResponse<string> response = await this.authService.Register(registerDto);
            if (!response.Success)
                return BadRequest(response);
            else
            {
                var confirmationLink = Url.ActionLink("ConfirmEmail", "Auth", new { email = registerDto.Email, token = response.Data });
                await this.smtpService.SendEmailAsync(this.configuration.GetSection("confirmEmailFromAddress").Value, registerDto.Email, "Confirm your Email Address", confirmationLink);
                return Ok();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            ServiceResponse<string> response = await this.authService.Login(loginDto);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            ServiceResponse<string> response = await this.authService.ConfirmEmail(email, token);
            if (!response.Success)
                return BadRequest(response);
            return Ok();
        }
    }
}