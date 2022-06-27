/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * 2022-06-15     KnownChen    App属性支持写入,不公开Init方法
 * 2022-06-26     KnownChen    移除GetCurrentApp方法
 * ------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Known.Core;

namespace Known
{
    public sealed class Config
    {
#if !NET6_0
        private const string ConfigPath = "Config.config";
#endif

        private Config() { }

        static Config()
        {
#if !NET6_0
            var path = Path.Combine(RootPath, ConfigPath);
            var json = File.ReadAllText(path);
            App = Utils.FromJson<AppInfo>(json);
            ContentRootPath = RootPath;
#endif
            IsInstalled = false;
            MacAddress = Utils.GetMacAddress();
        }

        public static string RootPath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        internal static bool IsInstalled { get; set; }
        internal static string WebRootPath { get; set; }
        internal static string ContentRootPath { get; set; }
        internal static string MacAddress { get; }
        public static AppInfo App { get; set; } = new AppInfo();
        internal static List<MenuInfo> Menus { get; private set; }
        internal static List<AppInfo> Apps { get; set; }

        internal static bool HasMenu
        {
            get { return Menus != null && Menus.Count > 0; }
        }

        internal static void Init()
        {
            var platform = new PlatformService();
            var info = platform.GetSystem();
            IsInstalled = info != null;
        }

        internal static void Init(AppInfo app)
        {
            App = app;
            Init();
        }

        public static Dictionary<string, string> GetExeSettings(string exePath)
        {
            var xmlFile = $"{exePath}.config";
            if (!File.Exists(xmlFile))
                return null;

            var settings = new Dictionary<string, string>();
            using (var tr = new XmlTextReader(xmlFile))
            {
                while (tr.Read())
                {
                    if (tr.NodeType == XmlNodeType.Element)
                    {
                        if (tr.Name == "add")
                        {
                            var key = tr.GetAttribute("key");
                            var value = tr.GetAttribute("value");
                            settings.Add(key, value);
                        }
                    }
                }
            }

            return settings;
        }
    }

    public class AppInfo
    {
        public string CompNo { get; set; }
        public string CompName { get; set; }
        public string AppId { get; set; }
        public string AppName { get; set; }
        public string AppUrl { get; set; }
        public string Description { get; set; }
        public string ProxyUrl { get; set; }
        public string UploadPath { get; set; }
        public Dictionary<string, object> Params { get; set; }
        public MailConfig Mail { get; set; }
        public List<ConnectionInfo> Connections { get; set; }
        public List<string> ServerTypes { get; set; }

        internal ConnectionInfo GetConnection(string name)
        {
            if (Connections == null || Connections.Count == 0)
                return null;

            return Connections.FirstOrDefault(c => c.Name == name);
        }

        public T Param<T>(string key, T defaultValue = default)
        {
            if (Params == null || Params.Count == 0)
                return defaultValue;

            if (!Params.ContainsKey(key))
                return defaultValue;

            var value = Params[key];
            if (typeof(T).IsClass)
                return Utils.MapTo<T>(value);

            return Utils.ConvertTo(Params[key], defaultValue);
        }
    }

    public class MailConfig
    {
        public string SmtpServer { get; set; }
        public int? SmtpPort { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string FromPassword { get; set; }
        public string ExceptionMails { get; set; }
    }

    public class ConnectionInfo
    {
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public string ProviderType { get; set; }
        public string ConnectionString { get; set; }
    }
}