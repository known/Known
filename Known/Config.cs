using System.Configuration;

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
    }
}
