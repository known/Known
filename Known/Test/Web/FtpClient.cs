using System;
using System.IO;
using System.Net;

namespace Known.Web
{
    /// <summary>
    /// Ftp 客户端操作类。
    /// </summary>
    public class FtpClient
    {
        private readonly string baseUrl = string.Empty;

        /// <summary>
        /// 初始化一个 Ftp 客户端操作类的实例。
        /// </summary>
        /// <param name="baseUrl">Ftp 服务器根地址。</param>
        public FtpClient(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        /// <summary>
        /// 取得或设置 Ftp 服务器用户名。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 取得或设置 Ftp 服务器用户密码。
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 上传文件到服务器。
        /// </summary>
        /// <param name="file">文件信息。</param>
        /// <param name="remoteFile">Ftp 文件地址。</param>
        /// <returns>上传成功返回 True，否则返回 False。</returns>
        public bool Upload(FileInfo file, string remoteFile)
        {
            return false;
        }

        /// <summary>
        /// 下载 Ftp 文件。
        /// </summary>
        /// <param name="remoteFile">Ftp 文件地址。</param>
        /// <returns>文件流字节。</returns>
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
