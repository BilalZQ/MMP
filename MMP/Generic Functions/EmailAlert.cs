using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace MMP.Generic_Functions
{
    public class EmailAlert
    {
        public static void SendEmail(string email, string body)
        {
            var fromEmail = new MailAddress("mmptimesheets@gmail.com", " MMP");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "Pass2work"; // Replace with Actual Password
            String subject = "MMP TimeSheets Alerts";
            

            /*var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            }) smtp.Send(message);*/
        }
    }
}