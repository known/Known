using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Known.Extensions;

namespace Known
{
    public sealed class Utils
    {
        #region Base
        public static string NewGuid
        {
            get { return Guid.NewGuid().ToString().ToLower().Replace("-", ""); }
        }

        public static bool IsNullOrEmpty(object value)
        {
            return value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString());
        }

        public static T ConvertTo<T>(object value, T defaultValue = default(T))
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

        public static string ToRmb(decimal value)
        {
            var s = value.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            var d = Regex.Replace(s, @"((?<=-|^)[^\-1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
            var rmb = "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟万亿兆京垓秭穰";
            var result = Regex.Replace(d, ".", m => rmb[m.Value[0] - '-'].ToString());
            if (result.EndsWith("元"))
                result = result + "整";

            return result;
        }

        public static string FromBase64String(string value)
        {
            var bytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string ToBase64String(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        public static string HideMobile(string mobile)
        {
            return Regex.Replace(mobile, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
        }

        public static decimal? Round(decimal? value, int decimals)
        {
            if (!value.HasValue)
                return null;

            return Math.Round(value.Value, decimals, MidpointRounding.AwayFromZero);
        }

        public static string GetUniqueString(int length = 8)
        {
            var str = "0123456789abcdefghijklmnopqrstuvwxyz";
            var chars = new char[length];
            var bytes = Guid.NewGuid().ToByteArray();
            for (var i = 0; i < length; i++)
            {
                chars[i] = str[(bytes[i] + bytes[bytes.Length - length + i]) % 35];
            }
            return new string(chars);
        }

        public static BackgroundWorker ExecuteAsync<T>(T context, Action<T, DoWorkEventArgs> doAction, Action<RunWorkerCompletedEventArgs> completeAction = null) where T : class
        {
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (o, e) =>
            {
                if (!(e.Argument is T ctx))
                    return;

                doAction?.Invoke(ctx, e);
            };
            if (completeAction != null)
            {
                backgroundWorker.RunWorkerCompleted += (o, e) =>
                {
                    completeAction?.Invoke(e);
                };
            }
            backgroundWorker.RunWorkerAsync(context);
            return backgroundWorker;
        }
        #endregion

        #region File
        public static bool ExistsFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            return File.Exists(fileName);
        }

        public static string EnsureDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public static string EnsureFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            return fileName;
        }

        public static void DeleteFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        public static void MoveFile(string sourceFileName, string destFileName)
        {
            EnsureFile(destFileName);
            DeleteFile(destFileName);
            File.Move(sourceFileName, destFileName);
        }

        public static string GetFileExtName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            var index = fileName.LastIndexOf('.');
            return fileName.Substring(index).ToLower();
        }
        #endregion

        #region IP
        public static string GetIPAddressName(string ipAddress)
        {
            return IPInfo.GetIPAddressName(ipAddress);
        }

        class IPInfo
        {
            class IPCheckResult
            {
                public int code { get; set; }
                public IPAdderssData data { get; set; }
            }

            class IPAdderssData
            {
                public string country { get; set; }
                public string country_id { get; set; }
                public string area { get; set; }
                public string area_id { get; set; }
                public string region { get; set; }
                public string region_id { get; set; }
                public string city { get; set; }
                public string city_id { get; set; }
                public string county { get; set; }
                public string county_id { get; set; }
                public string isp { get; set; }
                public string isp_id { get; set; }
                public string ip { get; set; }
            }

            private static Hashtable cached = new Hashtable();

            internal static string GetIPAddressName(string ipAddress)
            {
                if (!cached.ContainsKey(ipAddress))
                {
                    lock (cached.SyncRoot)
                    {
                        var ipAddressName = GetIPName(ipAddress);
                        if (!string.IsNullOrWhiteSpace(ipAddressName))
                            cached[ipAddress] = ipAddressName;
                    }
                }

                if (!cached.ContainsKey(ipAddress))
                    return string.Empty;

                return (string)cached[ipAddress];
            }

            private static string GetIPName(string ip)
            {
                try
                {
                    var url = string.Format("http://ip.taobao.com/service/getIpInfo.php?ip={0}", ip);
                    var client = new WebClient();
                    var json = client.DownloadString(url);
                    var info = json.FromJson<IPCheckResult>();
                    if (info == null)
                        return string.Empty;

                    var ipName = string.Empty;
                    if (info.data.region == info.data.city)
                        ipName = string.Format("{0}{1}{2}", info.data.region, info.data.county, info.data.isp);
                    else
                        ipName = string.Format("{0}{1}{2}{3}", info.data.region, info.data.city, info.data.county, info.data.isp);

                    if (string.IsNullOrWhiteSpace(ipName))
                        ipName = info.data.country;

                    return ipName;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        #endregion
    }
}

