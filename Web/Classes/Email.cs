using System.Collections.Generic;
using System.IO;
using System.Linq;
using GoC.TC.SecureMailer;
using GoC.TC.SecureMailer.Config;
using MailKit;
using MimeKit;


namespace Web.Classes
{
    public static class Email
    {
        public static bool Send(string subject, string body, string emailTo, string emailFrom, bool isHtml, List<EmailAttachment> attachments = null, string emailCc = "")
        {
            using (var mailer = new SecureSmtpClient(new DefaultConfigStrategy()))
            {
                //message body builder.
                var builder = new BodyBuilder();
                if (isHtml)
                {
                    builder.HtmlBody = body;
                }
                else
                {
                    builder.TextBody = body;
                }

                //create message.
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(emailFrom, emailFrom));
                message.Subject = subject;

                //add to mail addresses.
                foreach (var email in emailTo.Split(';'))
                {
                    if (!string.IsNullOrEmpty(email))
                        message.To.Add(new MailboxAddress(email, email));
                }
                if (!string.IsNullOrEmpty(emailCc))
                {
                    foreach (var cc in emailCc.Split(';'))
                    {
                        if (!string.IsNullOrEmpty(cc))
                            message.Cc.Add(new MailboxAddress(cc, cc));
                    }
                }

                //Attach one or more file(s)
                if (attachments != null)
                {
                    foreach (var a in attachments.Where(a =>
                        a.AttachmentBytes != null && a.AttachmentBytes.Length > 0 &&
                        !string.IsNullOrEmpty(a.AttachementName)))
                    {
                        builder.Attachments.Add(a.AttachementName, new MemoryStream(a.AttachmentBytes));
                    }
                }

                //set the message body.
                message.Body = builder.ToMessageBody();

                //send email.
                //mailer.NotifyDelivery = DeliveryStatusNotification.Failure | DeliveryStatusNotification.Delay | DeliveryStatusNotification.Success;
                mailer.SendMimeMessage(message);

                return true;
            }
        }
    }
    public class EmailAttachment
    {
        public string AttachementName { get; set; }
        public string AttachementType { get; set; }
        public Stream AttachementSream { get; set; }
        public byte[] AttachmentBytes { get; set; }
    }
}
