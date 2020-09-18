using System.Threading.Tasks;

namespace Dotnet_Core_3_API.Services.SMTP
{
    public interface ISmtpService
    {
        Task SendEmailAsync(string fromAddress, string toAddress, string subject, string message);
    }
}