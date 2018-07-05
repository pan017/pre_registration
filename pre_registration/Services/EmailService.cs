using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using System.Net;
using System;
using System.Net.Mail;

namespace pre_registration.Services
{
    public class EmailService
    {
        public static async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("mgaon", "ilya.panysh@mgaon.by"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("webmail.active.by", 25, false);
                await client.AuthenticateAsync("ilya.panysh@mgaon.by", "7632bxr29ZX6");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }

        }
        public static void SendMail(NotificationEmail fromMail, string toMail, string subject, string message)
        {
            try
            {
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("smtp.yandex.ru", 587);
                MailAddress from = new MailAddress(fromMail.Login, fromMail.DisplayngName);
                // кому отправляем
                MailAddress to = new MailAddress(toMail);
                // создаем объект сообщения
                MailMessage mailMessage = new MailMessage(from, to);
                // тема письма
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;

                mailMessage.Body = message;

                client.Credentials = new NetworkCredential(fromMail.Login, fromMail.Password);
                client.EnableSsl = true;
                client.Timeout = 30000;

                client.Send(mailMessage);
            }
            catch (Exception e)
            {

            }
        }
    }
}
