using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Known
{
    public sealed class Utils
    {
        #region Common
        public static T ConvertTo<T>(object value, T defaultValue = default)
        {
            return (T)ConvertTo(typeof(T), value, defaultValue);
        }

        public static object ConvertTo(Type type, object value, object defaultValue = null)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            var valueString = value.ToString();
            if (type == typeof(string))
                return Convert.ChangeType(valueString, type);

            valueString = valueString.Trim();
            if (valueString.Length == 0)
                return defaultValue;

            if (type.IsEnum)
                return Enum.Parse(type, valueString, true);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);

            if (type == typeof(bool) || type == typeof(bool?))
                valueString = ",1,Y,YES,TRUE,".Contains(valueString.ToUpper()) ? "True" : "False";

            try
            {
                return Convert.ChangeType(valueString, type);
            }
            catch
            {
                return defaultValue;
            }
        }
        #endregion

        #region Serialize
        public static string ToJson(object value, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (value == null)
                return string.Empty;

            var settings = new JsonSerializerSettings { DateFormatString = dateFormat };
            return JsonConvert.SerializeObject(value, settings);
        }

        public static T FromJson<T>(string json, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            var settings = new JsonSerializerSettings { DateFormatString = dateFormat };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static object FromJson(Type type, string json, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var settings = new JsonSerializerSettings { DateFormatString = dateFormat };
            return JsonConvert.DeserializeObject(json, type, settings);
        }
        #endregion

        #region Resource
        public static string GetResource(Assembly assembly, string name)
        {
            var text = string.Empty;
            if (assembly == null || string.IsNullOrEmpty(name))
                return text;

            var names = assembly.GetManifestResourceNames();
            name = names.FirstOrDefault(n => n.Contains(name));
            if (string.IsNullOrWhiteSpace(name))
                return text;

            var stream = assembly.GetManifestResourceStream(name);
            if (stream != null)
            {
                using (var sr = new StreamReader(stream))
                {
                    text = sr.ReadToEnd();
                }
            }
            return text;
        }
        #endregion
    }
}
