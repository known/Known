using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Known
{
    public class Mail
    {
        private List<MailAddress> tos = new List<MailAddress>();

        public Mail(string subject)
        {
        }

        public string Subject { get; set; }

        public string Body { get; set; }

        public void AddTo(MailAddress to)
        {

        }

        public void AddCc(MailAddress cc)
        {

        }

        public void Send()
        {

        }
    }
}
