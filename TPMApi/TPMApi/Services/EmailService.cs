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
            mimeMessage.From.Add(MailboxAddress.Parse("service@triplepromigrationapi.nl"));
            mimeMessage.To.Add(MailboxAddress.Parse(subject));
            mimeMessage.Subject = message;
            mimeMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = email
            };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.vevida.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("service@triplepromigrationapi.nl", "k$i0d(WEpF");
            smtp.Send(mimeMessage);
            smtp.Disconnect(true);
        }
    }
}
