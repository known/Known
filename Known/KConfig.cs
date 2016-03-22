using Known.Extensions;
using System.Configuration;

namespace Known
{
    public class KConfig
    {
        public static T GetSetting<T>(string key, T nullValue)
        {
            var value = ConfigurationManager.AppSettings[key].To<T>();
            if (value == null)
            {
                return nullValue;
            }
            return value;
        }
    }
}
