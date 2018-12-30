using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using Known.Log;
using Known.Validation;

namespace Known
{
    public class Mail
    {
        private readonly string smtpServer = string.Empty;
        private readonly int? smtpPort = null;
        private readonly string fromName = string.Empty;
        private readonly string fromEmail = string.Empty;
        private readonly string fromPassword = string.Empty;
        private List<MailAddress> toMails = new List<MailAddress>();
        private List<MailAddress> ccMails = new List<MailAddress>();
        private List<MailAddress> bccMails = new List<MailAddress>();
        private List<string> attachments = new List<string>();

        public Mail(string smtpServer, int? smtpPort, string fromName, string fromEmail, string fromPassword)
        {
            this.smtpServer = smtpServer;
            this.smtpPort = smtpPort;
            this.fromName = fromName;
            this.fromEmail = fromEmail;
            this.fromPassword = fromPassword;
        }

        public void AddTo(string email)
        {
            if (!Validator.IsEmail(email))
                return;

            if (!toMails.Exists(m => m.Address == email))
            {
                toMails.Add(new MailAddress(email));
            }
        }

        public void AddTo(string name, string email)
        {
            if (!Validator.IsEmail(email))
                return;

            if (!toMails.Exists(m => m.Address == email))
            {
                toMails.Add(new MailAddress(email, name));
            }
        }

        public void AddCc(string email)
        {
            if (!Validator.IsEmail(email))
                return;

            if (!ccMails.Exists(m => m.Address == email))
            {
                ccMails.Add(new MailAddress(email));
            }
        }

        public void AddCc(string name, string email)
        {
            if (!Validator.IsEmail(email))
                return;

            if (!ccMails.Exists(m => m.Address == email))
            {
                ccMails.Add(new MailAddress(email, name));
            }
        }

        public void AddBcc(string email)
        {
            if (!Validator.IsEmail(email))
                return;

            if (!bccMails.Exists(m => m.Address == email))
            {
                bccMails.Add(new MailAddress(email));
            }
        }

        public void AddBcc(string name, string email)
        {
            if (!Validator.IsEmail(email))
                return;

            if (!bccMails.Exists(m => m.Address == email))
            {
                bccMails.Add(new MailAddress(email, name));
            }
        }

        public void AddAttachment(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            if (!attachments.Contains(fileName))
            {
                attachments.Add(fileName);
            }
        }

        public void Send(string subject, string body, bool isBodyHtml = true)
        {
            if (toMails.Count == 0)
                return;

            if (!string.IsNullOrEmpty(smtpServer))
            {
                using (var client = smtpPort.HasValue
                                  ? new SmtpClient(smtpServer, smtpPort.Value)
                                  : new SmtpClient(smtpServer))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(fromEmail, fromPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    Send(subject, body, isBodyHtml, client);
                }
            }
            else
            {
                using (var client = new SmtpClient())
                {
                    Send(subject, body, isBodyHtml, client);
                }
            }

            toMails.Clear();
            ccMails.Clear();
            bccMails.Clear();
            attachments.Clear();
        }

        public static void Send(string toMails, string subject, string body, List<string> attachments = null, bool isBodyHtml = true)
        {
            if (string.IsNullOrWhiteSpace(toMails))
                return;

            var smtpServer = Config.AppSetting("SmtpServer");
            var smtpPort = Config.AppSetting<int?>("SmtpPort");
            var fromName = Config.AppSetting("FromName");
            var fromEmail = Config.AppSetting("FromEmail");
            var fromPassword = Config.AppSetting("FromPassword");
            var mail = new Mail(smtpServer, smtpPort, fromName, fromEmail, fromPassword);
            var tos = toMails.Split(';', '；');
            foreach (var item in tos)
            {
                mail.AddTo(item);
            }
            if (attachments != null && attachments.Count > 0)
            {
                attachments.ForEach(a => mail.AddAttachment(a));
            }
            mail.Send(subject, body, isBodyHtml);
        }

        public static void Send(string subject, Exception ex)
        {
            var exceptionMails = Config.AppSetting("ExceptionMails");
            if (string.IsNullOrWhiteSpace(exceptionMails))
            {
                WriteError(subject, ex.ToString());
                return;
            }

            Send(exceptionMails, subject, ex.ToString(), null, false);
        }

        private void Send(string subject, string body, bool isBodyHtml, SmtpClient client)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(fromEmail, fromName);
                    toMails.ForEach(m => message.To.Add(m));
                    ccMails.ForEach(m => message.CC.Add(m));
                    bccMails.ForEach(m => message.Bcc.Add(m));
                    message.Subject = subject;
                    message.Body = body;
                    message.BodyEncoding = Encoding.UTF8;
                    message.IsBodyHtml = isBodyHtml;
                    client.Send(message);
                }
            }
            catch
            {
                WriteError($"Send Fail: {subject}", body);
            }
        }

        private static void WriteError(string subject, string message)
        {
            var log = new TraceLogger(Environment.CurrentDirectory);
            log.Error($"{subject}{Environment.NewLine}{message}");
        }
    }
}
