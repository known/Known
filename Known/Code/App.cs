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

    public class AppContext
    {
        private static readonly Dictionary<string, object> session = new Dictionary<string, object>();

        public static AppContext Current => Container.Resolve(new AppContext());

        public virtual bool IsMobile { get; set; }
        
        public virtual string Host
        {
            get { return "localhost"; }
        }

        public virtual string GetIPAddress()
        {
            return string.Empty;
        }

        public virtual BrowserInfo GetBrowser()
        {
            return new BrowserInfo { Browser = "Console" };
        }

        public virtual string GetRequest(string key)
        {
            return string.Empty;
        }

        public virtual T GetCookie<T>(string key)
        {
            if (!session.ContainsKey(key))
                return default;

            return (T)session[key];
        }

        public virtual void SetCookie(string key, object value)
        {
            session[key] = value;
        }

        public virtual string GetSessionId()
        {
            return "local";
        }

        public virtual T GetSession<T>(string key)
        {
            if (!session.ContainsKey(key))
                return default;

            return (T)session[key];
        }

        public virtual void SetSession(string key, object value)
        {
            session[key] = value;
        }

        public virtual void ClearSession()
        {
            session.Clear();
        }

        public virtual List<IAttachFile> GetFormFiles()
        {
            return null;
        }
    }

    public class BrowserInfo
    {
        public string Platform { get; set; }
        public string Type { get; set; }
        public string Browser { get; set; }
        public string MajorVersion { get; set; }
        public string Version { get; set; }
    }
}