using CaseHandler.WebApplication.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CaseHandler.WebApplication.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ApplicationConfiguration _applicationConfiguration;
        private readonly MailAddress _fromAddress;
        private readonly SmtpClient _smtpClient;
        private readonly string _fromPassword;

        public EmailSender(ApplicationConfiguration applicationConfiguration)
        {
            _applicationConfiguration = applicationConfiguration;
            _fromPassword = _applicationConfiguration.ApplicationAdminEmailPassword;
            _fromAddress = new MailAddress(_applicationConfiguration.ApplicationAdminEmailAddress,
                _applicationConfiguration.ApplicationAdminEmailDisplayName);
            _smtpClient = new SmtpClient
            {
                Host = _applicationConfiguration.ApplicationAdminEmailSmtpHost,
                Port = _applicationConfiguration.ApplicationAdminEmailSmtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_fromAddress.Address, _fromPassword)
            };
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailMessage mailMessage = CreateMailMessage(email, subject, htmlMessage);

            using (mailMessage)
            {
                await _smtpClient.SendMailAsync(mailMessage);
            }
        }

        private MailMessage CreateMailMessage(string email, string subject, string htmlMessage)
        {
            MailMessage mailMessage = new MailMessage
            {
                From = _fromAddress,
                Subject = subject,
                IsBodyHtml = true,
                Body = htmlMessage
            };
            mailMessage.To.Add(email);

            return mailMessage;
        }
    }
}