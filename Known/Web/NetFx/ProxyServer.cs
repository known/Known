#if !NET6_0
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Known.Web
{
    public class ProxyRule
    {
        public string Pattern { get; set; }
        public string ProxyUrl { get; set; }
    }

    internal class ProxyServer
    {
        private readonly List<ProxyRule> _rules;
        private HttpContext _context;

        internal ProxyServer()
        {
            _rules = Config.App.Param<List<ProxyRule>>("ProxyRules");
        }

        internal void Execute(HttpContext context)
        {
            if (_rules == null || _rules.Count == 0)
                return;

            _context = context;
            var rawUrl = _context.Request.RawUrl;
            foreach (var item in _rules)
            {
                if (Regex.IsMatch(rawUrl, item.Pattern))
                {
                    if (item.ProxyUrl.StartsWith("http"))
                    {
                        var proxyUrl = ConvertProxyUrl(_context, item.ProxyUrl);
                        ExecuteProxy(proxyUrl);
                    }
                    else if (item.ProxyUrl.StartsWith("msg:"))
                    {
                        _context.Response.Write(item.ProxyUrl.Substring(4));
                        _context.Response.End();
                    }
                    else
                    {
                        _context.Response.Redirect(item.ProxyUrl);
                    }
                }
            }
        }

        private static string ConvertProxyUrl(HttpContext context, string proxyUrl)
        {
            if (string.IsNullOrEmpty(proxyUrl))
                return string.Empty;

            if (proxyUrl.Contains("$cookie_"))
            {
                var name = proxyUrl.Split('_')[1];
                var value = context.Request.Cookies[name].Value;
                if (!string.IsNullOrEmpty(value))
                {
                    proxyUrl = proxyUrl.Replace($"$cookie_{name}", value);
                }
            }

            return proxyUrl;
        }

        private void ExecuteProxy(string proxyUrl)
        {
            if (string.IsNullOrEmpty(proxyUrl))
                return;

            var remoteUrl = proxyUrl.TrimEnd('/') + _context.Request.RawUrl;
            var request = GetRequest(remoteUrl);
            var response = GetResponse(request);
            var responseData = GetResponseStreamBytes(response);
            _context.Response.ContentEncoding = Encoding.UTF8;
            _context.Response.ContentType = response.ContentType;
            _context.Response.OutputStream.Write(responseData, 0, responseData.Length);
            SetContextCookies(response);
            response.Close();
            _context.Response.End();
        }

        private HttpWebRequest GetRequest(string remoteUrl)
        {
            var cookieContainer = new CookieContainer();

            // Create a request to the server
            var request = (HttpWebRequest)WebRequest.Create(remoteUrl);

            // Set some options
            request.Method = _context.Request.HttpMethod;
            request.UserAgent = _context.Request.UserAgent;
            request.KeepAlive = true;
            request.CookieContainer = cookieContainer;

            // Send Cookie extracted from the incoming request
            for (int i = 0; i < _context.Request.Cookies.Count; i++)
            {
                var navigatorCookie = _context.Request.Cookies[i];
                var value = navigatorCookie.Value ?? "";
                value = value.Replace(",", "%2C");
                var cookie = new Cookie(navigatorCookie.Name, value);
                cookie.Domain = new Uri(remoteUrl).Host;
                cookie.Expires = navigatorCookie.Expires;
                cookie.HttpOnly = navigatorCookie.HttpOnly;
                cookie.Path = navigatorCookie.Path;
                cookie.Secure = navigatorCookie.Secure;
                cookieContainer.Add(cookie);
            }

            // For POST, write the post data extracted from the incoming request
            if (request.Method == "POST")
            {
                var clientStream = _context.Request.InputStream;
                var clientPostData = new byte[_context.Request.InputStream.Length];
                clientStream.Read(clientPostData, 0, (int)_context.Request.InputStream.Length);

                request.ContentType = _context.Request.ContentType;
                request.ContentLength = clientPostData.Length;
                var stream = request.GetRequestStream();
                stream.Write(clientPostData, 0, clientPostData.Length);
                stream.Close();
            }

            return request;
        }

        private HttpWebResponse GetResponse(HttpWebRequest request)
        {
            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (System.Net.WebException)
            {
                // Send 404 to client 
                _context.Response.StatusCode = 404;
                _context.Response.StatusDescription = "ReverseProxy Page Not Found";
                _context.Response.Write("ReverseProxy Page not found");
                _context.Response.End();
                return null;
            }

            return response;
        }

        private byte[] GetResponseStreamBytes(HttpWebResponse response)
        {
            var bufferSize = 256;
            var buffer = new byte[bufferSize];
            Stream responseStream;
            var memoryStream = new MemoryStream();
            int remoteResponseCount;
            byte[] responseData;

            responseStream = response.GetResponseStream();
            remoteResponseCount = responseStream.Read(buffer, 0, bufferSize);

            while (remoteResponseCount > 0)
            {
                memoryStream.Write(buffer, 0, remoteResponseCount);
                remoteResponseCount = responseStream.Read(buffer, 0, bufferSize);
            }

            responseData = memoryStream.ToArray();

            memoryStream.Close();
            responseStream.Close();

            memoryStream.Dispose();
            responseStream.Dispose();

            return responseData;
        }

        private void SetContextCookies(HttpWebResponse response)
        {
            _context.Response.Cookies.Clear();

            foreach (Cookie receivedCookie in response.Cookies)
            {
                var cookie = new HttpCookie(receivedCookie.Name, receivedCookie.Value);
                cookie.Domain = _context.Request.Url.Host;
                cookie.Expires = receivedCookie.Expires;
                cookie.HttpOnly = receivedCookie.HttpOnly;
                cookie.Path = receivedCookie.Path;
                cookie.Secure = receivedCookie.Secure;
                _context.Response.Cookies.Add(cookie);
            }
        }
    }
}
#endif