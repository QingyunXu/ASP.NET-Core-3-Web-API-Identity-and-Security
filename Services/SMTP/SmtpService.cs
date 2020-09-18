using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Dotnet_Core_3_API.Dto.SMTP;
using Microsoft.Extensions.Options;

namespace Dotnet_Core_3_API.Services.SMTP
{
    public class SmtpService : ISmtpService
    {
        private readonly IOptions<SmtpOptions> options;
        public SmtpService(IOptions<SmtpOptions> options)
        {
            this.options = options;
        }

        public async Task SendEmailAsync(string fromAddress, string toAddress, string subject, string message)
        {
            var mailMessage = new MailMessage(fromAddress, toAddress, subject, message);
            using (var client = new SmtpClient(this.options.Value.Host, this.options.Value.Port)
            { Credentials = new NetworkCredential(this.options.Value.Username, this.options.Value.Password) })
            {
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}