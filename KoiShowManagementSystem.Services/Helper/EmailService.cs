using KoiShowManagementSystem.DTOs.Request;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services.Helper
{
    public class EmailService : IEmailService
    {
        EmailConfig _emailConfig;
        public EmailService(IOptions<EmailConfig> emailConfig)
        {
            _emailConfig = emailConfig.Value;
        }
        public void SendEmail(EmailModel emailRequest)
        {
            try
            {
                SmtpClient smtpClient
                    = new SmtpClient(_emailConfig.Provider, _emailConfig.Port);
                smtpClient.Credentials
                    = new NetworkCredential(_emailConfig.DefaultSender, _emailConfig.Password);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.EnableSsl = true;


                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(_emailConfig.DefaultSender!, _emailConfig.DisplayName);
                mailMessage.To.Add(emailRequest.To!);
                mailMessage.Subject = emailRequest.Subject;
                mailMessage.Body = emailRequest.Content;
                mailMessage.IsBodyHtml = true;

                if (emailRequest.Attachment!.Length > 0)
                {
                    foreach (var path in emailRequest.Attachment)
                    {
                        Attachment attach = new Attachment(path);
                        mailMessage.Attachments.Add(attach);
                    }
                }

                smtpClient.Send(mailMessage);
            }
            catch
            {
                throw;
            }
        }
    }
}
