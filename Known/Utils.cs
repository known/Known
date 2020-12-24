using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Known
{
    public sealed class Utils
    {
        private Utils() { }

        #region Common
        public static string GetGuid()
        {
            return Guid.NewGuid().ToString().ToLower().Replace("-", "");
        }

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

        #region Encryptor
        public static string ToMd5(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            byte[] bytes;
            using (var md5 = MD5.Create())
            {
                var buffer = Encoding.UTF8.GetBytes(value);
                bytes = md5.ComputeHash(buffer);
            }

            var sb = new StringBuilder();
            foreach (var item in bytes)
            {
                sb.Append(item.ToString("x2"));
            }

            return sb.ToString();
        }
        #endregion

        #region Serialize
        public static string ToJson(object value)
        {
            if (value == null)
                return string.Empty;

            return JsonSerializer.Serialize(value);
        }

        public static T FromJson<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }

        public static object FromJson(Type type, string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonSerializer.Deserialize(json, type);
        }

        public static T MapTo<T>(object value)
        {
            if (value == null)
                return default;

            var json = JsonSerializer.Serialize(value);
            return JsonSerializer.Deserialize<T>(json);
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

        public static string GetFileContent(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            if (!File.Exists(path))
                return string.Empty;

            return File.ReadAllText(path);
        }

        public static void SaveFileContent(string path, string content)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            if (string.IsNullOrWhiteSpace(content))
                return;

            var info = new FileInfo(path);
            if (!info.Directory.Exists)
                info.Directory.Create();

            File.WriteAllText(path, content);
        }
        #endregion
    }
}
