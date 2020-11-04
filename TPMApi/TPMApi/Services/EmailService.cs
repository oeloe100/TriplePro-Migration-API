using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace TPMApi.Services
{
    public class EmailService : IEmailService
    {
        public void Send(string subject, string message, string email)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse("tonraschenko@hotmail.com"));
            mimeMessage.To.Add(MailboxAddress.Parse(subject));
            mimeMessage.Subject = message;
            mimeMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = email
            };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("wilford.hagenes@ethereal.email", "fGtaxqCP8zc1HNjdsd");
            smtp.Send(mimeMessage);
            smtp.Disconnect(true);
        }
    }
}
