/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

using System.Collections.Generic;
using Known.Core;
#if NET6_0
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
#else
using System.Web;
#endif

namespace Known.Web
{
    public class WebAppContext : AppContext
    {
#if NET6_0
        public static IServiceCollection Services;

        internal static HttpContext Context
        {
            get
            {
                var factory = Services.BuildServiceProvider().GetService(typeof(IHttpContextAccessor));
                var context = ((HttpContextAccessor)factory).HttpContext;
                return context;
            }
        }
#else
        private HttpContext Context
        {
            get { return HttpContext.Current; }
        }
#endif

        internal static string GetHost(HttpContext context)
        {
            if (context == null || context.Request == null)
                return string.Empty;

#if NET6_0
            var host = context.Request.Host;
            return host.Port == 80 ? host.Host : $"{host.Host}:{host.Port}";
#else
            var url = context.Request.Url;
            return url.Port == 80 ? url.Host : $"{url.Host}:{url.Port}";
#endif
        }

        private bool isMobile;
        public override bool IsMobile
        {
            get
            {
                if (Context == null || Context.Request == null)
                    return false;

                isMobile = WebUtils.CheckMobile(Context.Request);
                return isMobile;
            }
            set { isMobile = value; }
        }

        public override string Host
        {
            get { return GetHost(Context); }
        }

        public override string GetIPAddress()
        {
            if (Context == null || Context.Request == null)
                return string.Empty;

#if NET6_0
            return Context.Connection.RemoteIpAddress.ToString();
#else
            var request = Context.Request;
            var result = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(result))
                result = request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(result))
                result = request.UserHostAddress;

            if (string.IsNullOrEmpty(result) || result == "::1")
                return "127.0.0.1";

            return result;
#endif
        }

        public override BrowserInfo GetBrowser()
        {
            if (Context == null || Context.Request == null)
                return null;

            var request = Context.Request;
#if NET6_0
            var platform = "Win NT";
            var agent = request.Headers[HeaderNames.UserAgent];
#else
            var platform = request.Browser.Platform;
            var agent = request.UserAgent;
#endif
            if (agent.Contains("Android"))
                platform = "Android";
            else if (agent.Contains("iPhone"))
                platform = "iPhone";
            else if (agent.Contains("iPad"))
                platform = "iPad";
            else if (agent.Contains("Windows Phone"))
                platform = "Windows Phone";

#if NET6_0
            return new BrowserInfo
            {
                Platform = platform
            };
#else
            return new BrowserInfo
            {
                Platform = platform,
                Type = request.Browser.Type,
                Browser = request.Browser.Browser,
                MajorVersion = request.Browser.MajorVersion.ToString(),
                Version = request.Browser.Version
            };
#endif
        }

        public override string GetRequest(string key)
        {
            if (Context == null || Context.Request == null)
                return string.Empty;

            var value = Context.Request.Headers[key];
            if (!string.IsNullOrEmpty(value))
                return value;
#if NET6_0
            return Context.Request.Query[key];
#else
            return Context.Request.QueryString[key];
#endif
        }

        public override T GetCookie<T>(string key)
        {
            if (Context == null || Context.Request == null || Context.Request.Cookies == null)
                return default;

            var cookie = Context.Request.Cookies[key];
            if (cookie == null)
                return default;

#if NET6_0
            Context.Request.Cookies.TryGetValue(key, out string value);
            return Utils.ConvertTo<T>(value);
#else
            return Utils.ConvertTo<T>(cookie.Value);
#endif
        }

        public override void SetCookie(string key, object value)
        {
            if (Context == null || Context.Request == null || Context.Response.Cookies == null)
                return;

#if NET6_0
            Context.Response.Cookies.Append(key, value.ToString());
#else
            var cookie = new HttpCookie(key, value.ToString());
            Context.Response.Cookies.Set(cookie);
#endif
        }

        public override string GetSessionId()
        {
            if (Context == null || Context.Session == null)
                return string.Empty;

#if NET6_0
            return Context.Session.Id;
#else
            return Context.Session.SessionID;
#endif
        }

        public override T GetSession<T>(string key)
        {
            if (Context == null || Context.Session == null)
                return default;

#if NET6_0
            var json = Context.Session.GetString(key);
            return Utils.FromJson<T>(json);
#else
            return (T)Context.Session[key];
#endif
        }

        public override void SetSession(string key, object value)
        {
            if (Context == null || Context.Session == null)
                return;

#if NET6_0
            Context.Session.SetString(key, Utils.ToJson(value));
#else
            Context.Session[key] = value;
#endif
        }

        public override void ClearSession()
        {
            if (Context == null || Context.Session == null)
                return;

            Context.Session.Clear();
        }

        public override List<IAttachFile> GetFormFiles()
        {
            if (Context == null)
                return null;

            var files = new List<IAttachFile>();
#if NET6_0
            foreach (var item in Context.Request.Form.Files)
            {
                files.Add(new WebAttachFile(item));
            }
#else
            foreach (HttpPostedFile item in Context.Request.Files)
            {
                files.Add(new WebAttachFile(item));
            }
#endif
            return files;
        }
    }

    class WebAttachFile : IAttachFile
    {
#if NET6_0
        private readonly IFormFile file;

        public WebAttachFile(IFormFile file)
        {
            this.file = file;
            Length = file.Length;
            FileName = file.FileName;
        }
#else
        private readonly HttpPostedFile file;

        public WebAttachFile(HttpPostedFile file)
        {
            this.file = file;
            Length = file.ContentLength;
            FileName = file.FileName;
        }
#endif

        public long Length { get; }
        public string FileName { get; }

        public byte[] GetBytes()
        {
#if NET6_0
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                return ms.ToArray();
            }
#else
            return Utils.StreamToBytes(file.InputStream);
#endif
        }
    }
}