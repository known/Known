using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Known
{
    public class Mail
    {
        private string smtpServer = string.Empty;
        private string fromName = string.Empty;
        private string fromEmail = string.Empty;
        private string fromPassword = string.Empty;
        private List<MailAddress> toMails = new List<MailAddress>();
        private List<MailAddress> ccMails = new List<MailAddress>();
        private List<MailAddress> bccMails = new List<MailAddress>();

        public Mail(string fromName, string fromEmail)
        {
            this.fromName = fromName;
            this.fromEmail = fromEmail;
        }

        public Mail(string smtpServer, string fromName, string fromEmail, string fromPassword)
        {
            this.smtpServer = smtpServer;
            this.fromName = fromName;
            this.fromEmail = fromEmail;
            this.fromPassword = fromPassword;
        }

        public void AddTo(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (toMails.Count(m => m.Address == email) == 0)
            {
                toMails.Add(new MailAddress(email));
            }
        }

        public void AddTo(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (toMails.Count(m => m.Address == email) == 0)
            {
                toMails.Add(new MailAddress(email, name));
            }
        }

        public void AddCc(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (ccMails.Count(m => m.Address == email) == 0)
            {
                ccMails.Add(new MailAddress(email));
            }
        }

        public void AddCc(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (ccMails.Count(m => m.Address == email) == 0)
            {
                ccMails.Add(new MailAddress(email, name));
            }
        }

        public void AddBcc(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (bccMails.Count(m => m.Address == email) == 0)
            {
                bccMails.Add(new MailAddress(email));
            }
        }

        public void AddBcc(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (bccMails.Count(m => m.Address == email) == 0)
            {
                bccMails.Add(new MailAddress(email, name));
            }
        }

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

        private void Send(string subject, string body, bool isBodyHtml, SmtpClient client, MailMessage message)
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
}
