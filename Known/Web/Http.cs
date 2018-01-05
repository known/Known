using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Known.Web
{
    public class Http
    {
        private static string contentType = "application/x-www-form-urlencoded";
        private static string accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/x-silverlight, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-silverlight-2-b1, */*";
        private static string userAgent = "Mozilla/5.0(Windows NT 10.0;WOW64;Trident/7.0;rv:11.0) like Gecko";
        private static Encoding encoding = Encoding.GetEncoding("utf-8");
        private static int currentTry = 0;
        private static int maxTry = 3;

        public static string Get(string url, IDictionary<string, string> datas, out string cookie)
        {
            return DoRequest("GET", url, datas, out cookie);
        }

        public static string Post(string url, IDictionary<string, string> datas, out string cookie)
        {
            return DoRequest("POST", url, datas, out cookie);
        }

        public static string Get(string url, bool allowRedirect = false, IDictionary<string, string> datas = null, CookieContainer cookie = null)
        {
            return DoRequest("GET", url, allowRedirect, datas, cookie);
        }

        public static string Post(string url, IDictionary<string, string> datas = null, CookieContainer cookie = null)
        {
            return DoRequest("POST", url, false, datas, cookie);
        }

        public static string DownloadFile(string fileName, string method, string url, IDictionary<string, string> datas = null, CookieContainer cookie = null)
        {
            currentTry++;
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            try
            {
                req = GetHttpWebRequest(method, false, url, datas, cookie);
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

        private static HttpWebRequest GetHttpWebRequest(string method, bool allowRedirect, string url, IDictionary<string, string> datas = null, CookieContainer cookie = null)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            if (cookie != null && cookie.Count > 0)
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

        private static string DoRequest(string method, string url, IDictionary<string, string> datas, out string cookie)
        {
            currentTry++;
            HttpWebRequest req = null;
            HttpWebResponse res = null;
            cookie = "";

            try
            {
                req = GetHttpWebRequest(method, false, url, datas);
                res = (HttpWebResponse)req.GetResponse();

                var stream = res.GetResponseStream();
                var reader = new StreamReader(stream, encoding);
                var html = reader.ReadToEnd();
                reader.Close();
                stream.Close();

                currentTry--;
                req.Abort();
                res.Close();
                cookie = res.Headers.Get("Set-Cookie");
                return html;
            }
            catch
            {
                if (req != null)
                    req.Abort();
                if (res != null)
                    res.Close();

                return string.Empty;
            }
        }

        private static string DoRequest(string method, string url, bool allowRedirect = false, IDictionary<string, string> datas = null, CookieContainer cookie = null)
        {
            currentTry++;
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            try
            {
                req = GetHttpWebRequest(method, allowRedirect, url, datas, cookie);
                res = (HttpWebResponse)req.GetResponse();

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
