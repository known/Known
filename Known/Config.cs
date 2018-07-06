using System.Configuration;
using Known.Data;

namespace Known
{
    public sealed class Config
    {
        public const string DefaultConnectionName = "Default";

        public static string AppSetting(string key, string defaultValue = null)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return value;
        }

        public static T AppSetting<T>(string key, T defaultValue = default(T))
        {
            var value = AppSetting(key);
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return Utils.ConvertTo<T>(value);
        }

        public static Database GetDatabase()
        {
            var provider = Container.Load<IDbProvider>();
            if (provider == null)
                provider = new DefaultDbProvider(DefaultConnectionName);

            return new Database(provider);
        }
    }
}
