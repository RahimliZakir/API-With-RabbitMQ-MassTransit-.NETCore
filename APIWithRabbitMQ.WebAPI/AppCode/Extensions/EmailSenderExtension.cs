using System.Net;
using System.Net.Mail;

namespace APIWithRabbitMQ.WebAPI.AppCode.Extensions
{
    public static partial class EExtension
    {
        public static bool SendMail(this IConfiguration configuration, string fromMail, string password, string toMail, string subject, string body, bool isHtml = false, string? cc = null, int port = 25, string host = "smtp.mail.ru", bool ssl = true)
        {
            try
            {
                SmtpClient client = new()
                {
                    Host = host,
                    EnableSsl = ssl,
                    Port = port
                };

                client.Credentials = new NetworkCredential(fromMail, password);

                MailMessage message = new(fromMail, toMail);

                message.Subject = subject;
                message.Body = body;

                if (!string.IsNullOrWhiteSpace(cc))
                {
                    message.CC.Add(cc);
                }

                message.IsBodyHtml = isHtml;

                client.Send(message);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
