using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Apple.Snooper
{
    public class Mailer
    {
        public async Task<bool> Notify(EmailConfig emailConfig, string model, string store)
        {
            try
            {
                using (var emailClient = new SmtpClient())
                {
                    emailClient.Timeout = 10000;
                    emailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    emailClient.UseDefaultCredentials = false;
                    emailClient.Credentials = new NetworkCredential(emailConfig.From, emailConfig.Password);
                    emailClient.Host = emailConfig.Smtp;
                    emailClient.Port = 587;
                    emailClient.EnableSsl = true;
                    var message = string.Format(Constants.MensajeEmail, model, store);
                    var mailMessage = GetEmailMessage(emailConfig.From, emailConfig.Destinations, message);
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    
                    await emailClient.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static MailMessage GetEmailMessage(string from, IEnumerable<string> tos, string message)
        {
            var toList = tos.ToList();

            var mailMessage = new MailMessage(from, from, "iPhone available", message);
            foreach (var to in toList)
            {
                var ma = new MailAddress(to);
                mailMessage.CC.Add(ma);
            }


            return mailMessage;

        }
    }
}
