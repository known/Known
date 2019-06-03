using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Known.Web
{
    /// <summary>
    /// Http 客户端类。
    /// </summary>
    public class HttpClient
    {
        private static readonly string contentType = "application/x-www-form-urlencoded";
        private static readonly string accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/x-silverlight, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-silverlight-2-b1, */*";
        private static readonly string userAgent = "Mozilla/5.0(Windows NT 10.0;WOW64;Trident/7.0;rv:11.0) like Gecko";
        private static readonly Encoding encoding = Encoding.GetEncoding("utf-8");
        private static readonly int maxTry = 3;
        private static int currentTry = 0;
        private readonly CookieContainer cookie = new CookieContainer();
        private readonly string baseUrl = string.Empty;

        /// <summary>
        /// 初始化一个 Http 客户端类的实例。
        /// </summary>
        /// <param name="baseUrl">请求根地址。</param>
        public HttpClient(string baseUrl)
        {
            this.baseUrl = baseUrl;
            Get(baseUrl);
        }

        /// <summary>
        /// 设置客户端 Cookie 值。
        /// </summary>
        /// <param name="name">Cookie 名。</param>
        /// <param name="value">Cookie 值。</param>
        public void SetCookie(string name, string value)
        {
            cookie.Add(new Uri(baseUrl), new Cookie(name, value));
        }

        /// <summary>
        /// GET 请求操作。
        /// </summary>
        /// <param name="path">请求路径。</param>
        /// <param name="allowRedirect">是否允许直接跳转。</param>
        /// <returns>GET 到的请求内容。</returns>
        public string Get(string path, bool allowRedirect = false)
        {
            return DoRequest("GET", path, allowRedirect);
        }

        /// <summary>
        /// POST 请求操作。
        /// </summary>
        /// <param name="path">请求路径。</param>
        /// <param name="datas">请求的数据。</param>
        /// <returns>POST 请求返回的结果。</returns>
        public string Post(string path, IDictionary<string, string> datas = null)
        {
            return DoRequest("POST", path, false, datas);
        }

        /// <summary>
        /// 下载文件。
        /// </summary>
        /// <param name="fileName">文件保存的路径。</param>
        /// <param name="method">请求方式。</param>
        /// <param name="path">请求路径。</param>
        /// <param name="datas">请求参数。</param>
        /// <returns>返回空字符串表示下载成功，否则提示下载失败信息。</returns>
        public string DownloadFile(string fileName, string method, string path, IDictionary<string, string> datas = null)
        {
            currentTry++;
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            try
            {
                req = GetHttpWebRequest(method, false, path, datas);
                res = (HttpWebResponse)req.GetResponse();

                var fileLength = res.ContentLength;
                var stream = res.GetResponseStream();
                var reader = new StreamReader(stream, encoding);
                var bufferbyte = new byte[fileLength];
                int allByte = bufferbyte.Length;
                int startByte = 0;
                while (fileLength > 0)
                {
                    int downByte = stream.Read(bufferbyte, startByte, allByte);
                    if (downByte == 0) { break; };
                    startByte += downByte;
                    allByte -= downByte;
                }

                var file = new FileInfo(fileName);
                if (!file.Directory.Exists)
                    file.Directory.Create();

                var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
                fs.Write(bufferbyte, 0, bufferbyte.Length);
                reader.Close();
                stream.Close();
                fs.Close();

                currentTry--;
                req.Abort();
                res.Close();
                return string.Empty;
            }
            catch (Exception ex)
            {
                if (req != null)
                    req.Abort();
                if (res != null)
                    res.Close();

                return ex.Message;
            }
        }

        private HttpWebRequest GetHttpWebRequest(string method, bool allowRedirect, string path, IDictionary<string, string> datas = null)
        {
            var url = path.StartsWith("http") ? path : baseUrl + path;
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = cookie;
            req.AllowAutoRedirect = allowRedirect;
            req.ContentType = contentType;
            req.ServicePoint.ConnectionLimit = maxTry;
            req.Referer = url;
            req.Accept = accept;
            req.UserAgent = userAgent;
            req.Method = method;

            if (datas != null && datas.Count > 0)
            {
                var values = new List<string>();
                foreach (var item in datas)
                {
                    values.Add(string.Format("{0}={1}", item.Key, item.Value));
                }
                var dataString = string.Join("&", values.ToArray());
                var bytes = Encoding.Default.GetBytes(dataString);
                req.ContentLength = bytes.Length;
                var stream = req.GetRequestStream();
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }

            return req;
        }

        private string DoRequest(string method, string path, bool allowRedirect = false, IDictionary<string, string> datas = null)
        {
            currentTry++;
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            try
            {
                req = GetHttpWebRequest(method, allowRedirect, path, datas);
                res = (HttpWebResponse)req.GetResponse();
                if (res.Cookies != null && res.Cookies.Count > 0)
                    cookie.Add(res.Cookies);

                var stream = res.GetResponseStream();
                var reader = new StreamReader(stream, encoding);
                var html = reader.ReadToEnd();
                reader.Close();
                stream.Close();

                currentTry--;
                req.Abort();
                res.Close();
                return html;
            }
            catch (Exception ex)
            {
                if (req != null)
                    req.Abort();
                if (res != null)
                    res.Close();

                return ex.ToString();
            }
        }
    }
}
