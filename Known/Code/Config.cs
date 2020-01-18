using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;

namespace Known
{
    /// <summary>
    /// 应用程序默认配置文件操作类。
    /// </summary>
    public sealed class Config
    {
        /// <summary>
        /// 获取当前程序默认配置文件的 AppSettingsSection 数据。
        /// </summary>
        /// <param name="key">配置key。</param>
        /// <param name="defaultValue">未配置时的默认值。</param>
        /// <returns>配置数据。</returns>
        public static string AppSetting(string key, string defaultValue = null)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return value;
        }

        /// <summary>
        /// 获取当前程序默认配置文件的 AppSettingsSection 泛型数据。
        /// </summary>
        /// <typeparam name="T">配置数据的泛型。</typeparam>
        /// <param name="key">配置key。</param>
        /// <param name="defaultValue">未配置时的默认值。</param>
        /// <returns>配置的泛型数据。</returns>
        public static T AppSetting<T>(string key, T defaultValue = default)
        {
            var value = AppSetting(key);
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return Utils.ConvertTo<T>(value);
        }

        /// <summary>
        /// 获取指定exe程序配置文件的 AppSettingsSection 数据字典。
        /// </summary>
        /// <param name="exePath">exe程序路径。</param>
        /// <returns>程序配置数据字典。</returns>
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
