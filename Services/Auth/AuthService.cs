using System;
using System.Linq;
using System.Threading.Tasks;
using Dotnet_Core_3_API.Dto.Auth;
using Microsoft.AspNetCore.Identity;

namespace Dotnet_Core_3_API.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> userManager;

        public AuthService(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<ServiceResponse<string>> ConfirmEmail(string email, string token)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();
            try
            {
                var user = await this.userManager.FindByEmailAsync(email);
                var result = await this.userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    response.Success = false;
                    response.Message = string.Join("", result.Errors.Select(e => e.Description));
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<string>> Login(LoginDto loginDto)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();
            try { }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<string>> Register(RegisterDto registerDto)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();
            try
            {
                if (!await UserExists(registerDto.Email))
                {
                    var user = new IdentityUser { Email = registerDto.Email, UserName = registerDto.Email };
                    var result = await this.userManager.CreateAsync(user, registerDto.Password);
                    if (result.Succeeded)
                    {
                        user = await this.userManager.FindByEmailAsync(registerDto.Email);
                        string token = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                        response.Data = token;
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = string.Join("", result.Errors.Select(e => e.Description));
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "User already exists.";
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<bool> UserExists(string email)
        {
            var user = await this.userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            return true;
        }
    }
}