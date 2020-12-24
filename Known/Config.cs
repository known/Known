using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Known
{
    public sealed class Config
    {
        private Config() { }

        static Config()
        {
            var path = Path.Combine(RootPath, "config.json");
            var json = File.ReadAllText(path);
            App = Utils.FromJson<AppInfo>(json);
        }

        public static string RootPath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        public static AppInfo App { get; }

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
        public string AppId { get; set; }
        public string AppName { get; set; }
        public string CompNo { get; set; }
        public string CompName { get; set; }
        public List<ConnectionInfo> Connections { get; set; }

        internal ConnectionInfo GetConnection(string name)
        {
            if (Connections == null || Connections.Count == 0)
                return null;

            return Connections.FirstOrDefault(c => c.Name == name);
        }
    }

    public class ConnectionInfo
    {
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public string ConnectionString { get; set; }
    }
}
