using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using TPMApi.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace TPMApi.Services
{
    public class EmailSender : IEmailSender
    {
        //set only via Secret Manager
        public AuthMessageSenderOptions Options { get; }

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
        }

        /// <summary>
        /// Send Verification email after registration using SENDGRID
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);

            var msg = new SendGridMessage()
            {
                From = new EmailAddress("tonraschenko@hotmail.com", Options.SendGridUser),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };

            msg.AddTo(new EmailAddress(email));

            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}
