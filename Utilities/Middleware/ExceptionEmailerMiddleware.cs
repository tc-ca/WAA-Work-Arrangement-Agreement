using GoC.TC.SecureMailer;
using GoC.TC.SecureMailer.Config;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Utilities.Middleware
{
    public class ExceptionEmailerMiddleware
    {
        private readonly SecureSmtpClient _emailClient;
        private readonly IConfiguration _config;

        private string sender;
        private string recipient_to;
        public string BaseURL;
        public bool isTesting;

        public ExceptionEmailerMiddleware(IConfiguration config)
        {
            _config = config;
            _emailClient = new SecureSmtpClient(new DefaultConfigStrategy());
        }

        public async Task Invoke(HttpContext context)
        {
            Exception e = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            isTesting = (_config["EmailSettings:RedirectEmails"].ToString() == "True");

            if (e != null)
            {
                string innerExceptionDtls = "", formDtls = "";

                // user information
                string userName = context.Session.GetString("Username") ?? "Unknown";
                string displayName = context.Session.GetString("DisplayName") ?? "Unknown";
                string email = context.Session.GetString("Email") ?? "Unknown";
                string phoneNumber = context.Session.GetString("PhoneNumber") ?? "Unknown";
                string path = context.Request.Path.Value ?? "Unknown";
                string method = context.Request.Method ?? "Unknown";
                string host = context.Request.Host.Value ?? "Unknown";
                var builder = new BodyBuilder();

                string message = "The following unhandled error was generated in " + _config["AppSettings:AppAcronymEN"] + " on " + DateTime.Now
                    + LineBreak(2) + SubHeading("User Information") + "<ul>"
                    + "<li>User: " + displayName + " (" + userName + ")</li>"
                    + "<li>Email: " + email + "</li>"
                    + "<li>Phone number: " + phoneNumber + "</li>"
                    + "<li>Request path: " + path + "</li>"
                    + "<li>Method: " + method + "</li>"
                    + "<li>Host: " + host + "</li>"
                    + LineBreak(2) + SubHeading("Error Details") + "<ul>"
                    + "<li>Error type: " + e.GetType().ToString() + "</li>"
                    + "<li>Exception: " + e.Message + "</li>"
                    + "<li>Stack trace: " + (e.StackTrace != null ? e.StackTrace.Replace("\r\n", "<br />") : "") + "</li></ul>";
                if (e.InnerException != null)
                {
                    innerExceptionDtls = LineBreak(2) + SubHeading("Inner Exception Details")
                        + "<ul><li>Inner error type: " + e.InnerException.GetType().ToString() + "</li>"
                        + "<li>Inner exception: " + e.InnerException.Message + "</li>"
                        + "<li>Inner stack trace: " + (e.InnerException.StackTrace != null ? e.InnerException.StackTrace.Replace("\r\n", "<br />") : "") + "</li></ul>";
                }

                message += innerExceptionDtls + formDtls;
                builder.HtmlBody = message;


                var errorEmail = new MimeMessage()
                {
                    Subject = isTesting ? "Testing--" + $"Error / Erreur: Work Arrangement Agreement" : $"Error / Erreur: Work Arrangement Agreement",
                    Body = builder.ToMessageBody()
                };

                sender =  _config["EmailSettings:ErrorTo"].ToString();
                recipient_to =  _config["EmailSettings:ErrorTo"].ToString();

                errorEmail.From.Add(new MailboxAddress(sender, sender));
                errorEmail.To.Add(new MailboxAddress(recipient_to, recipient_to));
                _emailClient.SendMimeMessage(errorEmail);

                string culture = context.Request.Path.Value.Split('/')[1];
                string errorPath = (culture == "fr") ? $"/fr/Error" : "/en/Error";
                context.Response.Redirect(errorPath);
            }
        }

        private string LineBreak(int numLines = 1)
        {
            string result = "";

            for (int i = 1; i <= numLines; i++)
            {
                result += "<br />";
            }

            return result;
        }

        private string SubHeading(string text) { return "<h3>" + text + "</h3>"; }

    }
}
