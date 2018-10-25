using System.Configuration;

namespace Known
{
    public sealed class Config
    {
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
    }
}
