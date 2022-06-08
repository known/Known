using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Known
{
    public class Mail
    {
        private readonly List<MInfo> toMails;
        private readonly List<MInfo> ccMails;
        private readonly List<MInfo> bccMails;
        private readonly List<MFile> mailFiles;

        public Mail(string subject, string body, bool isBodyHtml = true)
        {
            toMails = new List<MInfo>();
            ccMails = new List<MInfo>();
            bccMails = new List<MInfo>();
            mailFiles = new List<MFile>();

            EnableSsl = false;
            Subject = subject;
            Body = body;
            IsBodyHtml = isBodyHtml;
        }

        public bool EnableSsl { get; set; }
        public string Subject { get; }
        public string Body { get; }
        public bool IsBodyHtml { get; }

        public void AddTo(string email, string name = null)
        {
            AddMail(toMails, email, name);
        }

        public void AddCc(string email, string name = null)
        {
            AddMail(ccMails, email, name);
        }

        public void AddBcc(string email, string name = null)
        {
            AddMail(bccMails, email, name);
        }

        public void AddFile(string name, string path)
        {
            if (!File.Exists(path))
                return;

            mailFiles.Add(new MFile(name, path));
        }

        public void AddFile(string name, Stream stream)
        {
            if (stream == null)
                return;

            mailFiles.Add(new MFile(name, stream));
        }

        public void Send()
        {
            var mail = Config.App.Mail;
            if (mail == null)
                throw new Exception("Mail is not config.");

            if (string.IsNullOrEmpty(mail.FromEmail) || toMails.Count == 0)
                return;

            try
            {
                var client = mail.SmtpPort.HasValue
                           ? new SmtpClient(mail.SmtpServer, mail.SmtpPort.Value)
                           : new SmtpClient(mail.SmtpServer);
                //using (client)
                using (var message = new MailMessage())
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(mail.FromEmail, mail.FromPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = EnableSsl;

                    message.From = new MailAddress(mail.FromEmail, mail.FromName);
                    toMails.ForEach(m => message.To.Add(new MailAddress(m.Address, m.Name)));
                    ccMails.ForEach(m => message.CC.Add(new MailAddress(m.Address, m.Name)));
                    bccMails.ForEach(m => message.Bcc.Add(new MailAddress(m.Address, m.Name)));
                    message.Subject = Subject;
                    message.Body = Body;
                    message.BodyEncoding = Encoding.UTF8;
                    message.IsBodyHtml = IsBodyHtml;
                    AddAttachments(message, mailFiles);
                    client.Send(message);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception("SendMail", ex);
            }
        }

        public static void Send(string subject, Exception ex)
        {
            var mail = Config.App.Mail;
            if (mail == null)
                throw new Exception("Mail is not config.");

            Send(mail.ExceptionMails, subject, ex.ToString(), false);
        }

        public static void Send(string toMails, string subject, string body, bool isBodyHtml = true)
        {
            if (string.IsNullOrEmpty(toMails))
                return;

            var info = new Mail(subject, body, isBodyHtml);
            var mails = toMails.Split(';', ',', '；', '，');
            foreach (var item in mails)
            {
                info.AddTo(item);
            }

            info.Send();
        }

        private static void AddAttachments(MailMessage message, List<MFile> mailFiles)
        {
            mailFiles.ForEach(f =>
            {
                var att = f.Stream != null
                        ? new Attachment(f.Stream, "")
                        : new Attachment(f.Path);
                if (!string.IsNullOrEmpty(f.Name))
                    att.Name = f.Name;
                message.Attachments.Add(att);
            });
        }

        private static void AddMail(List<MInfo> mails, string email, string name = null)
        {
            if (string.IsNullOrEmpty(email))
                return;

            if (string.IsNullOrEmpty(name))
                name = email.Split('@')[0];

            mails.Add(new MInfo(name, email));
        }

        class MInfo
        {
            internal MInfo(string name, string address)
            {
                Name = name;
                Address = address;
            }

            internal string Name { get; }
            internal string Address { get; }
        }

        class MFile
        {
            internal MFile(string name, string path)
            {
                Name = name;
                Path = path;
            }

            internal MFile(string name, Stream stream)
            {
                Name = name;
                Stream = stream;
            }

            internal string Name { get; }
            internal string Path { get; }
            internal Stream Stream { get; }
        }
    }
}