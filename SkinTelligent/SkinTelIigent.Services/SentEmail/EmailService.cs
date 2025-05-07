using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MimeKit;
using SkinTelIigent.Contracts.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SkinTelIigent.Services.SentEmail
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _smtpHost = _configuration["Smtp:Host"] ?? throw new ArgumentNullException("Smtp:Host is missing in configuration");
            _smtpPort = _configuration.GetValue<int>("Smtp:Port");
            _smtpUsername = _configuration["Smtp:Username"] ?? throw new ArgumentNullException("Smtp:Username is missing in configuration");
            _smtpPassword = _configuration["Smtp:Password"] ?? throw new ArgumentNullException("Smtp:Password is missing in configuration");
        }

        public async Task<bool> SendEmailAsync(string emailTo, string subject, string? body = null, IList<IFormFile>? attachments = null)
        {
            try
            {
                var email = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(_smtpUsername),
                    Subject = subject
                };
                email.To.Add(MailboxAddress.Parse(emailTo));

                var bodyBuilder = new BodyBuilder { HtmlBody = BuildEmailBody(body ?? "", subject) };

                if (attachments?.Count > 0)
                {
                    foreach (var file in attachments)
                    {
                        if (file.Length > 0)
                        {
                            await using var ms = new MemoryStream();
                            await file.CopyToAsync(ms);
                            ms.Position = 0;
                            bodyBuilder.Attachments.Add(file.FileName, ms.ToArray(), ContentType.Parse(file.ContentType));
                        }
                    }
                }

                email.Body = bodyBuilder.ToMessageBody();

                return await SendEmailUsingSmtpAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {EmailTo}", emailTo);
                return false;
            }
        }

        private string BuildEmailBody(string body, string subject)
        {
            return $@"
            <html>
                <head>
                    <style>
                        h1 {{ color: #535154; font-family: Arial, Helvetica, sans-serif; font-size: 24px; }}
                        .content {{ padding: 10px; background-color: #f9f9f9; color: #333; width: 100%; text-align: center; font-size: 16px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h1>{subject}</h1>
                        <div class='content'>{body}</div>
                    </div>
                </body>
            </html>";
        }

        private async Task<bool> SendEmailUsingSmtpAsync(MimeMessage email)
        {
            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_smtpUsername, _smtpPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMTP email sending failed.");
                return false;
            }
        }
    }
}
