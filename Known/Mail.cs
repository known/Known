using System;
using System.Collections.Generic;
using System.Linq;
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
        private string smtpServer = string.Empty;
        private string fromName = string.Empty;
        private string fromEmail = string.Empty;
        private string fromPassword = string.Empty;
        private List<MailAddress> toMails = new List<MailAddress>();
        private List<MailAddress> ccMails = new List<MailAddress>();
        private List<MailAddress> bccMails = new List<MailAddress>();

        /// <summary>
        /// 构造函数，创建一个邮件操作类实例。
        /// </summary>
        /// <param name="fromName">发送者名称。</param>
        /// <param name="fromEmail">发送者邮箱。</param>
        public Mail(string fromName, string fromEmail)
        {
            this.fromName = fromName;
            this.fromEmail = fromEmail;
        }

        /// <summary>
        /// 构造函数，创建一个邮件操作类实例。
        /// </summary>
        /// <param name="smtpServer">发送邮件服务器。</param>
        /// <param name="fromName">发送者名称。</param>
        /// <param name="fromEmail">发送者邮箱。</param>
        /// <param name="fromPassword">发送者邮箱密码。</param>
        public Mail(string smtpServer, string fromName, string fromEmail, string fromPassword)
        {
            this.smtpServer = smtpServer;
            this.fromName = fromName;
            this.fromEmail = fromEmail;
            this.fromPassword = fromPassword;
        }

        /// <summary>
        /// 添加收件人邮箱。
        /// </summary>
        /// <param name="email">收件人邮箱。</param>
        public void AddTo(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (toMails.Count(m => m.Address == email) == 0)
            {
                toMails.Add(new MailAddress(email));
            }
        }

        /// <summary>
        /// 添加收件人邮箱。
        /// </summary>
        /// <param name="name">收件人名称。</param>
        /// <param name="email">收件人邮箱。</param>
        public void AddTo(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (toMails.Count(m => m.Address == email) == 0)
            {
                toMails.Add(new MailAddress(email, name));
            }
        }

        /// <summary>
        /// 添加抄送人邮箱。
        /// </summary>
        /// <param name="email">抄送人邮箱。</param>
        public void AddCc(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (ccMails.Count(m => m.Address == email) == 0)
            {
                ccMails.Add(new MailAddress(email));
            }
        }

        /// <summary>
        /// 添加抄送人邮箱。
        /// </summary>
        /// <param name="name">抄送人名称。</param>
        /// <param name="email">抄送人邮箱。</param>
        public void AddCc(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (ccMails.Count(m => m.Address == email) == 0)
            {
                ccMails.Add(new MailAddress(email, name));
            }
        }

        /// <summary>
        /// 添加密送人邮箱。
        /// </summary>
        /// <param name="email">密送人邮箱。</param>
        public void AddBcc(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (bccMails.Count(m => m.Address == email) == 0)
            {
                bccMails.Add(new MailAddress(email));
            }
        }

        /// <summary>
        /// 添加密送人邮箱。
        /// </summary>
        /// <param name="name">密送人名称。</param>
        /// <param name="email">密送人邮箱。</param>
        public void AddBcc(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (bccMails.Count(m => m.Address == email) == 0)
            {
                bccMails.Add(new MailAddress(email, name));
            }
        }

        /// <summary>
        /// 发送邮件。
        /// </summary>
        /// <param name="subject">邮件主题。</param>
        /// <param name="body">邮件内容。</param>
        /// <param name="isBodyHtml">邮件内容格式是否为HTML。</param>
        public void Send(string subject, string body, bool isBodyHtml = true)
        {
            if (toMails.Count == 0)
                return;

            if (!string.IsNullOrEmpty(smtpServer))
            {
                using (var client = new SmtpClient(smtpServer))
                using (var message = new MailMessage())
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(fromEmail, fromPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    Send(subject, body, isBodyHtml, client, message);
                }
            }
            else
            {
                using (var client = new SmtpClient())
                using (var message = new MailMessage())
                {
                    Send(subject, body, isBodyHtml, client, message);
                }
            }

            toMails.Clear();
            ccMails.Clear();
            bccMails.Clear();
        }

        /// <summary>
        /// 使用默认配置邮件服务发送邮件。
        /// </summary>
        /// <param name="toMails">收件人邮箱，多个用逗号分隔。</param>
        /// <param name="subject">邮件主题。</param>
        /// <param name="body">邮件内容。</param>
        /// <param name="isBodyHtml">邮件内容格式是否为HTML。</param>
        public static void Send(string toMails, string subject, string body, bool isBodyHtml = true)
        {
            if (string.IsNullOrWhiteSpace(toMails))
                return;

            var smtpServer = Config.AppSetting("SmtpServer");
            var fromName = Config.AppSetting("FromName");
            var fromEmail = Config.AppSetting("FromEmail");
            var fromPassword = Config.AppSetting("FromPassword");
            var mail = new Mail(smtpServer, fromName, fromEmail, fromPassword);
            var tos = toMails.Split(',');
            foreach (var item in tos)
            {
                mail.AddTo(item);
            }
            mail.Send(subject, body, isBodyHtml);
        }

        /// <summary>
        /// 使用默认配置邮件服务发送异常邮件。
        /// </summary>
        /// <param name="subject">邮件主题。</param>
        /// <param name="ex">异常。</param>
        public static void Send(string subject, Exception ex)
        {
            var exceptionMails = Config.AppSetting("ExceptionMails");
            if (string.IsNullOrWhiteSpace(exceptionMails))
            {
                WriteError(subject, ex.ToString());
                return;
            }

            Send(exceptionMails, subject, ex.ToString(), false);
        }

        private void Send(string subject, string body, bool isBodyHtml, SmtpClient client, MailMessage message)
        {
            try
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
            catch
            {
                WriteError($"发送邮件{subject}失败", body);
            }
        }

        private static void WriteError(string subject, string message)
        {
            var log = new TraceLogger(Environment.CurrentDirectory);
            log.Error($"{subject}{Environment.NewLine}{message}");
        }
    }
}
