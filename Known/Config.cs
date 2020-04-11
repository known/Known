using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;

namespace Known
{
    public sealed class Config
    {
        static Config()
        {
            AppId = Config.AppSetting("AppId");
            AppName = Config.AppSetting("AppName");
            IsDebug = Config.AppSetting("IsDebug", false);
            SmtpServer = Config.AppSetting("SmtpServer");
            SmtpPort = Config.AppSetting<int>("SmtpPort");
            SmtpFromName = Config.AppSetting("SmtpFromName");
            SmtpFromEmail = Config.AppSetting("SmtpFromEmail");
            SmtpFromPassword = Config.AppSetting("SmtpFromPassword");
            ExceptionMails = Config.AppSetting("ExceptionMails");
        }

        public static string AppId { get; }
        public static string AppName { get; }
        public static bool IsDebug { get; }
        public static string SmtpServer { get; }
        public static int SmtpPort { get; }
        public static string SmtpFromName { get; }
        public static string SmtpFromEmail { get; }
        public static string SmtpFromPassword { get; }
        public static string ExceptionMails { get; set; }

        public static string AppSetting(string key, string defaultValue = null)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return value;
        }

        public static T AppSetting<T>(string key, T defaultValue = default)
        {
            var value = AppSetting(key);
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return Utils.ConvertTo<T>(value);
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
}
