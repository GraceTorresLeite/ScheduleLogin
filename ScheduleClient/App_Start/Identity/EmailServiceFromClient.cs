using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace ScheduleClient.App_Start.Identity
{
    public class EmailServiceFromClient : IIdentityMessageService
    {
        private readonly string EMAIL_ORIGIN = ConfigurationManager.AppSettings["emailService:email_sender"];
        private readonly string EMAIL_PASSWORD = ConfigurationManager.AppSettings["emailService:email_password"];
        public async Task SendAsync(IdentityMessage message)
        {
            //class do .Net que represent nosso e-mail
            using (var messageEmail = new MailMessage())
            {
                messageEmail.From = new MailAddress(EMAIL_ORIGIN);

                messageEmail.Subject = message.Subject;
                messageEmail.To.Add(message.Destination); // poderia mandar e-mail pra vários, i.e (por exemplo) tb pra sms
                messageEmail.Body = message.Body;

                //SMTP - SIMPLE MAIL TRANSPORT PROTOCOL
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(EMAIL_ORIGIN, EMAIL_PASSWORD);

                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.Host = "smtp.gmail.com";
                    smtpClient.Port = 587;
                    smtpClient.EnableSsl = true;

                    smtpClient.Timeout = 20000; 

                    await smtpClient.SendMailAsync(messageEmail);
                }
            }
        }
    }
}