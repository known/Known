using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Known.Models;

namespace Known
{
    public class KSettings
    {
        public static string Name
        {
            get { return GetSetting<string>("Name"); }
        }

        public static string Version
        {
            get { return GetSetting<string>("Version"); }
        }

        public static T GetSetting<T>(string code)
        {
            var settings = GetSettings();
            var setting = settings.FirstOrDefault(s => s.Code == code);
            return setting != null ? setting.Value.To<T>() : default(T);
        }

        private static List<SettingInfo> settings;
        public static List<SettingInfo> GetSettings()
        {
            if (settings == null)
            {
                settings = AppContext.SettingService.GetSettings();
            }
            return settings;
        }

        public static ValidateResult UpdateSettings(List<SettingInfo> settings)
        {
            return AppContext.SettingService.UpdateSettings(settings);
        }
    }
}
