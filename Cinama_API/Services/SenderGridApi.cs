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
            var apiKey = "SG._xEiJ5GSayz7vbtZRRe-g.5UIsjNpFyA_LakyUK-8HSW-kcrJZOepbtWR7Mpohaf0";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("test@example.com", "Mohammad haj");
            var to = new EmailAddress(UserEmail, UserName);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            return await Task.FromResult(true);

        }
    }
}
