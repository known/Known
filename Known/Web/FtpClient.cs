using System;
using System.IO;
using System.Net;

namespace Known.Web
{
    public class FtpClient
    {
        private readonly string baseUrl = string.Empty;

        public FtpClient(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public string UserName { get; set; }
        public string Password { get; set; }

        public bool Upload(FileInfo file, string remoteFile)
        {
            return false;
        }

        public byte[] Download(string remoteFile)
        {
            var url = baseUrl + remoteFile;
            var request = CreateConnection(url, WebRequestMethods.Ftp.DownloadFile);
            return null;
        }

        private FtpWebRequest CreateConnection(string url, string method)
        {
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
            request.Credentials = new NetworkCredential(UserName, Password);
            //request.Proxy = this.proxy;
            request.KeepAlive = false;
            request.UseBinary = true;
            request.UsePassive = false;
            //request.EnableSsl = enableSsl;
            request.Method = method;
            return request;
        }
    }
}
