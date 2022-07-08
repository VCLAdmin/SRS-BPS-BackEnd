// using SendGrid's C# Library
// https://github.com/sendgrid/sendgrid-csharp
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VCLWebAPI.Utils;

namespace VCLWebAPI.Services
{
    public class IMailService
    {
        public IMailService( )
        {
        }

        public static async Task<Response> SendEmailAsync(string toEmail, string subject, string htmlContent, string header, string orderNumbers = null)
        {
           // string apikey = WebConfigurationManager.AppSettings["SENDGRID_API_KEY"];
            string apikey = Globals.SENDGRID_API_KEY; //System.Configuration.ConfigurationManager.AppSettings["SENDGRID_API_KEY"];
            var sendGridClient = new SendGridClient(apikey);
             // var sendGridClient =  new SendGridClient(Environment.GetEnvironmentVariable("SENDGRID_API_KEY"));
            var sendGridMessage = new SendGridMessage();
            sendGridMessage.SetFrom("VCLDesignService@gmail.com", "Schuco USA");
            sendGridMessage.AddTo(toEmail);
           // sendGridMessage.SetSubject(subject);
            // sendGridMessage.Subject = subject;
            //The Template Id will be something like this - d-9416e4bc396e4e7fbb658900102abaa2
            sendGridMessage.SetTemplateId("d-3de6384288de43cbaa2ff149b477ca60");
            //Here is the Place holder values you need to replace.
            sendGridMessage.SetTemplateData(new
            {
                HeaderContent = header,
                MainContent = htmlContent,
                Subject = subject,
                OrderContent = orderNumbers
            });

           // sendGridMessage.SetSpamCheck(false);
            sendGridMessage.SetBypassSpamManagement(true);
            return await sendGridClient.SendEmailAsync(sendGridMessage);
        }

        public static async Task<Response> SendMultipleEmailAsync(List<EmailAddress> tos, string subject, string htmlContent, string header, string orderNumbers = null)
        {

            // var sendGridClient = new SendGridClient("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");
            //string apikey = WebConfigurationManager.AppSettings["SENDGRID_API_KEY"];
            string apikey = Globals.SENDGRID_API_KEY; //System.Configuration.ConfigurationManager.AppSettings["SENDGRID_API_KEY"];
            var sendGridClient = new SendGridClient(apikey);
            var sendGridMessage = new SendGridMessage();
            sendGridMessage.SetFrom("VCLDesignService@gmail.com", "Schuco USA");
            sendGridMessage.AddTos(tos);
            // sendGridMessage.SetSubject(subject);
            // sendGridMessage.Subject = subject;
            //The Template Id will be something like this - d-9416e4bc396e4e7fbb658900102abaa2
            sendGridMessage.SetTemplateId("d-3de6384288de43cbaa2ff149b477ca60");
            //Here is the Place holder values you need to replace.
            sendGridMessage.SetTemplateData(new
            {
                HeaderContent = header,
                MainContent = htmlContent,
                Subject = subject,
                OrderContent = orderNumbers
            });

            sendGridMessage.SetBypassSpamManagement(true);
            return await sendGridClient.SendEmailAsync(sendGridMessage);
        }
    }
}