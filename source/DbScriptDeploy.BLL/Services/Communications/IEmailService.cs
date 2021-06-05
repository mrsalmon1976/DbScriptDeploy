using DbScriptDeploy.BLL.Models.Communications;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Services.Communications
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailModel email);
    }

}
