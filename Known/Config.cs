using Known.Data;
using System;
using System.Configuration;

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
        /// <returns>key对应的value值。</returns>
        public static string AppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// 根据key获取config文件AppSetting值并转换成指定的类型。
        /// </summary>
        /// <typeparam name="T">value值转换类型。</typeparam>
        /// <param name="key">配置key。</param>
        /// <returns>key对应的value值。</returns>
        public static T AppSetting<T>(string key)
        {
            var value = AppSetting(key);
            return Utils.ConvertTo<T>(value);
        }

        /// <summary>
        /// 获取config文件中默认数据库对象。
        /// </summary>
        /// <returns>数据库对象。</returns>
        public static Database GetDatabase()
        {
            return GetDatabase(DefaultConnectionName);
        }

        /// <summary>
        /// 获取config文件中指定name的数据库对象。
        /// </summary>
        /// <param name="name">数据库链接名称。</param>
        /// <returns>数据库对象。</returns>
        public static Database GetDatabase(string name)
        {
            var setting = ConfigurationManager.ConnectionStrings[name];
            var typeName = string.Format("Known.Data.{0}Database", setting.ProviderName);
            var type = Type.GetType(typeName);
            if (type == null)
                throw new Exception($"暂不支持{setting.ProviderName}数据访问组件！");

            var database = Activator.CreateInstance(type, setting.ConnectionString) as IDatabase;
            if (database == null)
                throw new Exception($"{setting.ProviderName}数据库组件未继承IDatabase接口！");

            return new Database(database);
        }
    }
}
