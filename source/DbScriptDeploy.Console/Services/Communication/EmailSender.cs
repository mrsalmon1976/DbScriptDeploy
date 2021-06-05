using DbScriptDeploy.BLL.Models.Communications;
using DbScriptDeploy.BLL.Services.Communications;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DbScriptDeploy.Console.Services.Communication
{
    public class EmailSender : IEmailSender
    {
        private IEmailService _emailService;

        public EmailSender(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            EmailModel emailModel = new EmailModel();
            emailModel.Test = email;
            await _emailService.SendEmailAsync(emailModel);
            System.Diagnostics.Debug.WriteLine($"EmailSender.SendEmailAsync: {email}");
        }
    }
}
