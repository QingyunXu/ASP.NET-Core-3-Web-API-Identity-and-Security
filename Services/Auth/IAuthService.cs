using System.Threading.Tasks;
using Dotnet_Core_3_API.Dto.Auth;

namespace Dotnet_Core_3_API.Services.Auth
{
    public interface IAuthService
    {
        Task<ServiceResponse<string>> Register(RegisterDto registerDto);
        Task<ServiceResponse<string>> Login(LoginDto loginDto);
        Task<bool> UserExists(string email);
        Task<ServiceResponse<string>> ConfirmEmail(string email, string token);
    }
}