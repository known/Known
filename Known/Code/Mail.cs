using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using Known.Log;

namespace Known
{
    /// <summary>
    /// 邮件操作类。
    /// </summary>
    public class Mail
    {
        private readonly string smtpServer = string.Empty;
        private readonly int? smtpPort = null;
        private readonly string fromName = string.Empty;
        private readonly string fromEmail = string.Empty;
        private readonly string fromPassword = string.Empty;
        private readonly List<MailAddress> toMails = new List<MailAddress>();
        private readonly List<MailAddress> ccMails = new List<MailAddress>();
        private readonly List<MailAddress> bccMails = new List<MailAddress>();
        private readonly List<Attachment> attachments = new List<Attachment>();

        /// <summary>
        /// 初始化一个邮件操作类实例。
        /// </summary>
        /// <param name="smtpServer">邮件 SMTP 服务器。</param>
        /// <param name="smtpPort">邮件 SMTP 服务器端口。</param>
        /// <param name="fromName">发送者名称。</param>
        /// <param name="fromEmail">发送者邮箱。</param>
        /// <param name="fromPassword">发送者邮箱密码。</param>
        public Mail(string smtpServer, int? smtpPort, string fromName, string fromEmail, string fromPassword)
        {
            this.smtpServer = smtpServer;
            this.smtpPort = smtpPort;
            this.fromName = fromName;
            this.fromEmail = fromEmail;
            this.fromPassword = fromPassword;
        }

        /// <summary>
        /// 添加一个收件人邮箱。
        /// </summary>
        /// <param name="email">收件人邮箱。</param>
        public void AddTo(string email)
        {
            if (!Validator.IsEmail(email))
                return;

            if (!toMails.Exists(m => m.Address == email))
            {
                toMails.Add(new MailAddress(email));
            }
        }

        /// <summary>
        /// 添加一个收件人邮箱。
        /// </summary>
        /// <param name="name">收件人名称。</param>
        /// <param name="email">收件人邮箱。</param>
        public void AddTo(string name, string email)
        {
            if (!Validator.IsEmail(email))
                return;

            if (!toMails.Exists(m => m.Address == email))
            {
                toMails.Add(new MailAddress(email, name));
            }
        }

        /// <summary>
        /// 添加一个抄送人邮箱。
        /// </summary>
        /// <param name="email">抄送人邮箱。</param>
        public void AddCc(string email)
        {
            if (!Validator.IsEmail(email))
                return;

            if (!ccMails.Exists(m => m.Address == email))
            {
                ccMails.Add(new MailAddress(email));
            }
        }

        /// <summary>
        /// 添加一个抄送人邮箱。
        /// </summary>
        /// <param name="name">抄送人名称。</param>
        /// <param name="email">抄送人邮箱。</param>
        public void AddCc(string name, string email)
        {
            if (!Validator.IsEmail(email))
                return;

            if (!ccMails.Exists(m => m.Address == email))
            {
                ccMails.Add(new MailAddress(email, name));
            }
        }

        /// <summary>
        /// 添加一个密送人邮箱。
        /// </summary>
        /// <param name="email">密送人邮箱。</param>
        public void AddBcc(string email)
        {
            if (!Validator.IsEmail(email))
                return;

            if (!bccMails.Exists(m => m.Address == email))
            {
                bccMails.Add(new MailAddress(email));
            }
        }

        /// <summary>
        /// 添加一个密送人邮箱。
        /// </summary>
        /// <param name="name">密送人名称。</param>
        /// <param name="email">密送人邮箱。</param>
        public void AddBcc(string name, string email)
        {
            if (!Validator.IsEmail(email))
                return;

            if (!bccMails.Exists(m => m.Address == email))
            {
                bccMails.Add(new MailAddress(email, name));
            }
        }

        /// <summary>
        /// 添加一个邮件附件。
        /// </summary>
        /// <param name="fileName">附件文件路径。</param>
        public void AddAttachment(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            attachments.Add(new Attachment(fileName));
        }

        /// <summary>
        /// 添加一个邮件附件。
        /// </summary>
        /// <param name="stream">附件文件流。</param>
        /// <param name="name">附件显示的文件名。</param>
        public void AddAttachment(Stream stream, string name)
        {
            if (stream == null)
                return;

            attachments.Add(new Attachment(stream, name));
        }

        /// <summary>
        /// 发送邮件。
        /// </summary>
        /// <param name="subject">邮件主题。</param>
        /// <param name="body">邮件内容。</param>
        /// <param name="isBodyHtml">邮件内容是否为 Html，默认是。</param>
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

        /// <summary>
        /// 发送邮件。
        /// </summary>
        /// <param name="toMails">收件人邮箱，多个用分号(;)隔开。</param>
        /// <param name="subject">邮件主题。</param>
        /// <param name="body">邮件内容。</param>
        /// <param name="attachments">邮件附件集合，默认空。</param>
        /// <param name="isBodyHtml">邮件内容是否为 Html，默认是。</param>
        public static void Send(string toMails, string subject, string body, Dictionary<string, Stream> attachments = null, bool isBodyHtml = true)
        {
            if (string.IsNullOrWhiteSpace(toMails))
                return;

            var smtpServer = Setting.Instance.SmtpServer;
            var smtpPort = Setting.Instance.SmtpPort;
            var fromName = Setting.Instance.SmtpFromName;
            var fromEmail = Setting.Instance.SmtpFromEmail;
            var fromPassword = Setting.Instance.SmtpFromPassword;
            var mail = new Mail(smtpServer, smtpPort, fromName, fromEmail, fromPassword);
            var tos = toMails.Split(';', '；');
            foreach (var item in tos)
            {
                mail.AddTo(item);
            }
            if (attachments != null && attachments.Count > 0)
            {
                foreach (var item in attachments)
                {
                    mail.AddAttachment(item.Value, item.Key);
                }
            }
            mail.Send(subject, body, isBodyHtml);
        }

        /// <summary>
        /// 发送异常邮件。
        /// </summary>
        /// <param name="subject">邮件主题。</param>
        /// <param name="ex">异常对象。</param>
        public static void Send(string subject, Exception ex)
        {
            var exceptionMails = Setting.Instance.ExceptionMails;
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
                    attachments.ForEach(m => message.Attachments.Add(m));
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
