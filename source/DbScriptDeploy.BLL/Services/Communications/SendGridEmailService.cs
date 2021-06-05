using DbScriptDeploy.BLL.Models.Communications;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Services.Communications
{

    public class SendGridEmailService : IEmailService
    {
        public async Task SendEmailAsync(EmailModel email)
        {

            await Task.Delay(1000);
            System.Diagnostics.Debug.WriteLine("SendGridEmailService.SendEmailAsync: " + email.Test);

            //const string apiKey = "DUMMY";
            //const string message = "DUMMY";
            //var client = new SendGridClient(apiKey);
            //var msg = new SendGridMessage()
            //{
            //    From = new EmailAddress("test@dbscriptdeploy.co.za", "TestUser"),
            //    Subject = "TEST",
            //    PlainTextContent = message,
            //    HtmlContent = message
            //};
            //msg.AddTo(new EmailAddress("someone@somewhere.com"));
            //msg.SetClickTracking(false, false);
            //return client.SendEmailAsync(msg);

        }
    }
}
