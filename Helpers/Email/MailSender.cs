using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace InventorySystem.Helpers.Email
{
    public static class MailSender
    {
        private static readonly string senderMail = "olabilirbilmem1@gmail.com";
        private static readonly string senderPassword = "gtcaxdkidocvmrlv";


        public static async Task SendEmailAsync(string toEmail ,string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderMail, senderPassword),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderMail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(toEmail);
                smtpClient.Send(mailMessage);

                Debug.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while sending the email: {ex.Message}");
            }
        }

    }
}
