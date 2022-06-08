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

namespace Known
{
    public interface IAppModule
    {
        void Initialize(AppInfo app);
    }

    public interface IAppContext
    {
        string Host { get; }
        bool CheckMobile();
        string GetIPAddress();
        BrowserInfo GetBrowser();
        string GetRequest(string key);
        T GetCookie<T>(string key);
        void SetCookie(string key, object value);
        string GetSessionId();
        T GetSession<T>(string key);
        void SetSession(string key, object value);
        void ClearSession();
        List<IAttachFile> GetFormFiles();
    }

    public class BrowserInfo
    {
        public string Platform { get; set; }
        public string Type { get; set; }
        public string Browser { get; set; }
        public string MajorVersion { get; set; }
        public string Version { get; set; }
    }

    class DefaultAppContext : IAppContext
    {
        private static readonly Dictionary<string, object> session = new Dictionary<string, object>();
        internal static IAppContext Current => Container.Resolve<IAppContext>(new DefaultAppContext());

        public string Host
        {
            get { return "localhost"; }
        }

        public bool CheckMobile()
        {
            return false;
        }

        public string GetIPAddress()
        {
            return string.Empty;
        }

        public BrowserInfo GetBrowser()
        {
            return new BrowserInfo { Browser = "Console" };
        }

        public string GetRequest(string key)
        {
            return string.Empty;
        }

        public T GetCookie<T>(string key)
        {
            if (!session.ContainsKey(key))
                return default;

            return (T)session[key];
        }

        public void SetCookie(string key, object value)
        {
            session[key] = value;
        }

        public string GetSessionId()
        {
            return "local";
        }

        public T GetSession<T>(string key)
        {
            if (!session.ContainsKey(key))
                return default;

            return (T)session[key];
        }

        public void SetSession(string key, object value)
        {
            session[key] = value;
        }

        public void ClearSession()
        {
            session.Clear();
        }

        public List<IAttachFile> GetFormFiles()
        {
            return null;
        }
    }
}