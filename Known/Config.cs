using System.Configuration;
using Known.Data;

namespace Known
{
    /// <summary>
    /// 配置文件操作类。
    /// </summary>
    public sealed class Config
    {
        /// <summary>
        /// 默认数据库链接名称。
        /// </summary>
        public const string DefaultConnectionName = "Default";

        /// <summary>
        /// 根据key获取config文件AppSetting值。
        /// </summary>
        /// <param name="key">配置key。</param>
        /// <param name="defaultValue">为空时的默认值。</param>
        /// <returns>key对应的value值。</returns>
        public static string AppSetting(string key, string defaultValue = null)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return value;
        }

        /// <summary>
        /// 根据key获取config文件AppSetting值并转换成指定的类型。
        /// </summary>
        /// <typeparam name="T">value值转换类型。</typeparam>
        /// <param name="key">配置key。</param>
        /// <param name="defaultValue">为空时的默认值。</param>
        /// <returns>key对应的value值。</returns>
        public static T AppSetting<T>(string key, T defaultValue = default(T))
        {
            var value = AppSetting(key);
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return Utils.ConvertTo<T>(value);
        }

        /// <summary>
        /// 获取config文件中默认数据库对象。
        /// </summary>
        /// <returns>数据库对象。</returns>
        public static Database GetDatabase()
        {
            var provider = Container.Load<IDbProvider>();
            if (provider == null)
                provider = new DefaultDbProvider(DefaultConnectionName);

            return new Database(provider);
        }
    }
}
