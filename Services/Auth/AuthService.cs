using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dotnet_Core_3_API.Dto.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Dotnet_Core_3_API.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;

        public AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.signInManager = signInManager;
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
            try
            {
                string issuers = this.configuration["Tokens:Issuer"];
                string audience = this.configuration["Tokens:Audience"];
                string key = this.configuration["Tokens:Key"];
                SignInResult signInResult = await this.signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, false);
                if (signInResult.Succeeded)
                {
                    var user = await this.userManager.FindByEmailAsync(loginDto.Email);

                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Email,user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti,user.Id),
                    };
                    SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    SigningCredentials credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);
                    JwtSecurityToken token = new JwtSecurityToken(issuers, audience, claims, expires: DateTime.Now.AddDays(1), signingCredentials: credentials);
                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    response.Data = tokenHandler.WriteToken(token);
                }
                else
                {
                    response.Success = false;
                    response.Message = "Invalid username or password.";
                }
            }
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