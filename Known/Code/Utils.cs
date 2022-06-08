using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;
#if NET6_0
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;
#else
using Newtonsoft.Json;
#endif

namespace Known
{
#if NET35
    public class DynamicObject : Dictionary<string, object>
    {
        public DynamicObject(Dictionary<string, object> dic)
        {
            if (dic.ContainsKey("Id"))
                Id = dic["Id"];
            if (dic.ContainsKey("HeadId"))
                HeadId = dic["HeadId"];

            this.Clear();
            foreach (var item in dic)
            {
                this[item.Key] = item.Value;
            }
        }

        public object Id { get; set; }
        public object HeadId { get; set; }
    }
#endif

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

        #region Round
        public static decimal Round(decimal value, int decimals)
        {
            return Math.Round(value, decimals, MidpointRounding.AwayFromZero);
        }

        public static double Round(double value, int decimals)
        {
            return Math.Round(value, decimals, MidpointRounding.AwayFromZero);
        }
        #endregion

        #region Encryptor
        public static string ToMd5(string value)
        {
            if (string.IsNullOrEmpty(value))
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

#if NET6_0
            return JsonSerializer.Serialize(value);
#else
            return JsonConvert.SerializeObject(value);
#endif
        }

#if NET35
        public static DynamicObject ToDynamic(string json)
        {
            var dic = FromJson<Dictionary<string, object>>(json);
            return new DynamicObject(dic);
        }
#endif

#if NET472
        public static dynamic ToDynamic(string json)
        {
            return FromJson<dynamic>(json);
        }
#endif

#if NET6_0
        public static dynamic ToDynamic(string json)
        {
            if (string.IsNullOrEmpty(json))
                return default;

            var dics = FromJson<Dictionary<string, object>>(json);
            var obj = new ExpandoObject();
            foreach (var item in dics)
            {
                obj.TryAdd(item.Key, item.Value?.ToString());
            }

            return obj;
        }

        private static readonly JsonSerializerOptions dsOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        private static string FormatDSJson(string json)
        {
            return json.Replace("{}", "null").Replace("\"\"", "null");
        }
#endif

        public static T FromJson<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                return default;

            try
            {
#if NET6_0
                json = FormatDSJson(json);
                return JsonSerializer.Deserialize<T>(json, dsOptions);
#else
                return JsonConvert.DeserializeObject<T>(json);
#endif
            }
            catch (Exception ex)
            {
                Logger.Exception(typeof(T).FullName, json, ex);
                return default;
            }
        }

        public static object FromJson(Type type, string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
#if NET6_0
                json = FormatDSJson(json);
                return JsonSerializer.Deserialize(json, type, dsOptions);
#else
                return JsonConvert.DeserializeObject(json, type);
#endif
            }
            catch (Exception ex)
            {
                Logger.Exception(type.FullName, json, ex);
                return default;
            }
        }

        public static T MapTo<T>(object value)
        {
            if (value == null)
                return default;

            if (value.GetType() == typeof(T))
                return (T)value;

            var json = ToJson(value);
            return FromJson<T>(json);
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
            if (string.IsNullOrEmpty(name))
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

        public static string ReadFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            if (!File.Exists(path))
                return string.Empty;

            return File.ReadAllText(path);
        }

        public static void SaveFile(string path, string content)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (string.IsNullOrEmpty(content))
                return;

            var info = new FileInfo(path);
            if (!info.Directory.Exists)
                info.Directory.Create();

            File.WriteAllText(path, content);
        }

        public static void SaveFile(string path, byte[] bytes)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (bytes == null || bytes.Length == 0)
                return;

            var info = new FileInfo(path);
            if (!info.Directory.Exists)
                info.Directory.Create();

            File.WriteAllBytes(path, bytes);
        }

        public static void DeleteFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (!File.Exists(path))
                return;

            File.Delete(path);
        }
        #endregion

        #region Color
        public static Color FromHtml(string htmlColor)
        {
            return ColorTranslator.FromHtml(htmlColor);
        }

        public static string ToHtml(Color color)
        {
            return ColorTranslator.ToHtml(color);
        }
        #endregion

        #region IP
        public static string GetMacAddress()
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            if (nics == null || nics.Length < 1)
                return string.Empty;

            foreach (var adapter in nics)
            {
                var ips = adapter.GetIPProperties();
                var addresses = ips.UnicastAddresses;
                foreach (var item in addresses)
                {
                    if (item.IsDnsEligible)
                    {
                        var address = adapter.GetPhysicalAddress();
                        var hexs = address.GetAddressBytes().Select(b => b.ToString("X2")).ToArray();
                        return string.Join(":", hexs);
                    }
                }
            }

            return string.Empty;
        }

        public static string GetLocalIPAddress()
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            if (nics == null || nics.Length < 1)
                return string.Empty;

            var ip = string.Empty;
            foreach (var adapter in nics)
            {
                var ips = adapter.GetIPProperties();
                var addresses = ips.UnicastAddresses;
                foreach (var item in addresses)
                {
                    if (item.IsDnsEligible)
                    {
                        ip = item.Address.ToString();
                    }
                }
            }

            return ip;
        }

        public static bool Ping(string host, int timeout = 120)
        {
            try
            {
                var ping = new Ping();
                var options = new PingOptions { DontFragment = true };
                var data = "";
                var buffer = Encoding.UTF8.GetBytes(data);
                var reply = ping.Send(host, timeout, buffer, options);
                return reply.Status == IPStatus.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region Byte
        public static byte[] StreamToBytes(Stream stream)
        {
            if (stream == null)
                return null;

            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        public static byte[] HexToBytes(string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
                return null;

            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";

            var bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Trim(), 16);
            }

            return bytes;
        }

        public static string BytesToHex(byte[] bytes, string separator = " ")
        {
            if (bytes == null || bytes.Length == 0)
                return string.Empty;

            var items = new List<string>();
            for (int i = 0; i < bytes.Length; i++)
            {
                items.Add(((int)bytes[i]).ToString("X2"));
            }

            return string.Join(separator, items.ToArray());
        }
        #endregion

        #region Pinyin
        public static string GetPinyin(string value)
        {
            string temp = "";
            int iLen = value.Length;
            for (var i = 0; i <= iLen - 1; i++)
            {
                temp += GetCharSpellCode(value.Substring(i, 1));
            }
            return temp;
        }

        private static string GetCharSpellCode(string value)
        {
            long iCnChar;
            byte[] ZW = Encoding.Default.GetBytes(value);

            if (ZW.Length == 1)
                return value.ToUpper();

            int i1 = ZW[0];
            int i2 = ZW[1];
            iCnChar = i1 * 256 + i2;

            if ((iCnChar >= 45217) && (iCnChar <= 45252))
                return "A";
            else if ((iCnChar >= 45253) && (iCnChar <= 45760))
                return "B";
            else if ((iCnChar >= 45761) && (iCnChar <= 46317))
                return "C";
            else if ((iCnChar >= 46318) && (iCnChar <= 46825))
                return "D";
            else if ((iCnChar >= 46826) && (iCnChar <= 47009))
                return "E";
            else if ((iCnChar >= 47010) && (iCnChar <= 47296))
                return "F";
            else if ((iCnChar >= 47297) && (iCnChar <= 47613))
                return "G";
            else if ((iCnChar >= 47614) && (iCnChar <= 48118))
                return "H";
            else if ((iCnChar >= 48119) && (iCnChar <= 49061))
                return "J";
            else if ((iCnChar >= 49062) && (iCnChar <= 49323))
                return "K";
            else if ((iCnChar >= 49324) && (iCnChar <= 49895))
                return "L";
            else if ((iCnChar >= 49896) && (iCnChar <= 50370))
                return "M";
            else if ((iCnChar >= 50371) && (iCnChar <= 50613))
                return "N";
            else if ((iCnChar >= 50614) && (iCnChar <= 50621))
                return "O";
            else if ((iCnChar >= 50622) && (iCnChar <= 50905))
                return "P";
            else if ((iCnChar >= 50906) && (iCnChar <= 51386))
                return "Q";
            else if ((iCnChar >= 51387) && (iCnChar <= 51445))
                return "R";
            else if ((iCnChar >= 51446) && (iCnChar <= 52217))
                return "S";
            else if ((iCnChar >= 52218) && (iCnChar <= 52697))
                return "T";
            else if ((iCnChar >= 52698) && (iCnChar <= 52979))
                return "W";
            else if ((iCnChar >= 52980) && (iCnChar <= 53640))
                return "X";
            else if ((iCnChar >= 53689) && (iCnChar <= 54480))
                return "Y";
            else if ((iCnChar >= 54481) && (iCnChar <= 55289))
                return "Z";

            return "?";
        }
        #endregion
    }
}