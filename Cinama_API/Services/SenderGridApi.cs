using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinama_API.Services
{
    public static class SenderGridApi
    {
        public static async Task<bool> Execute(string UserEmail,string UserName,string subject,string plainTextContent,string htmlContent)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("dev.haj94@gmail.com", "Mohammad haj");
            var to = new EmailAddress(UserEmail, UserName);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            return await Task.FromResult(true);

        }
    }
}
