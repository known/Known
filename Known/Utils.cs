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
    /// <summary>
    /// 系统效用类。
    /// </summary>
    public sealed class Utils
    {
        #region Base
        /// <summary>
        /// 取得一个新的全局唯一的 GUID 字符串。
        /// </summary>
        public static string NewGuid
        {
            get { return Guid.NewGuid().ToString().ToLower().Replace("-", ""); }
        }

        /// <summary>
        /// 判断对象是否为空或空白字符串。
        /// </summary>
        /// <param name="value">对象。</param>
        /// <returns>为空或空白返回 True，否则返回 False。</returns>
        public static bool IsNullOrEmpty(object value)
        {
            return value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString());
        }

        /// <summary>
        /// 将对象转换成指定泛型类型的对象。
        /// </summary>
        /// <typeparam name="T">转换目标类型。</typeparam>
        /// <param name="value">对象。</param>
        /// <param name="defaultValue">转换为空时的默认值。</param>
        /// <returns>指定泛型类型的对象。</returns>
        public static T ConvertTo<T>(object value, T defaultValue = default)
        {
            return (T)ConvertTo(typeof(T), value, defaultValue);
        }

        /// <summary>
        /// 将对象转换成指定类型的对象。
        /// 若目标类型为布尔类型，1,Y,YES,TRUE 均可转换为 True。
        /// </summary>
        /// <param name="type">转换目标类型。</param>
        /// <param name="value">对象。</param>
        /// <param name="defaultValue">转换为空时的默认值。</param>
        /// <returns>指定类型的对象。</returns>
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

        /// <summary>
        /// 将小写金额转为人民币大写金额。
        /// </summary>
        /// <param name="value">小写金额。</param>
        /// <returns>人民币大写金额。</returns>
        public static string ToRmb(decimal value)
        {
            var s = value.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            var d = Regex.Replace(s, @"((?<=-|^)[^\-1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
            var rmb = "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟万亿兆京垓秭穰";
            var result = Regex.Replace(d, ".", m => rmb[m.Value[0] - '-'].ToString());
            if (result.EndsWith("元"))
                result += "整";

            return result;
        }

        /// <summary>
        /// 将 Base64 编码字符串转为明文字符串。
        /// </summary>
        /// <param name="value">Base64 编码字符串。</param>
        /// <returns>明文字符串。</returns>
        public static string FromBase64String(string value)
        {
            var bytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// 将明文字符串转为 Base64 编码字符串。
        /// </summary>
        /// <param name="value">明文字符串。</param>
        /// <returns>Base64 编码字符串。</returns>
        public static string ToBase64String(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 隐藏手机号码中间4位数，用 * 替换。
        /// </summary>
        /// <param name="mobile">手机号码。</param>
        /// <returns>隐藏后的手机号码。</returns>
        public static string HideMobile(string mobile)
        {
            return Regex.Replace(mobile, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
        }

        /// <summary>
        /// 获取指定位数的四舍五入后的数值。
        /// </summary>
        /// <param name="value">数值。</param>
        /// <param name="decimals">保留的小数位数。</param>
        /// <returns>四舍五入后的数值。</returns>
        public static decimal? Round(decimal? value, int decimals)
        {
            if (!value.HasValue)
                return null;

            return Math.Round(value.Value, decimals, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 获取指定长度的唯一字符串，默认长度为8。
        /// </summary>
        /// <param name="length">长度。</param>
        /// <returns>唯一字符串。</returns>
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

        /// <summary>
        /// 在单独线程上执行异步操作。
        /// </summary>
        /// <typeparam name="T">执行上下文类型。</typeparam>
        /// <param name="context">执行上下文对象。</param>
        /// <param name="doAction">执行操作。</param>
        /// <param name="completeAction">执行完成操作。</param>
        /// <returns>单独线程上执行操作者。</returns>
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
        /// <summary>
        /// 判断是否存在指定的文件。
        /// </summary>
        /// <param name="fileName">文件路径。</param>
        /// <returns>存在返回 True，否则返回 False。</returns>
        public static bool ExistsFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            return File.Exists(fileName);
        }

        /// <summary>
        /// 确定指定文件夹路径存在，不存在则自动创建。
        /// </summary>
        /// <param name="path">文件夹路径。</param>
        /// <returns>文件夹路径。</returns>
        public static string EnsureDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        /// <summary>
        /// 确定指定文件路径存在，不存在则自动创建。
        /// </summary>
        /// <param name="fileName">文件路径。</param>
        /// <returns>文件路径。</returns>
        public static string EnsureFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            return fileName;
        }

        /// <summary>
        /// 删除指定的文件。
        /// </summary>
        /// <param name="fileName">文件路径。</param>
        public static void DeleteFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        /// <summary>
        /// 移动文件，若存在目的文件，则先删除再移动。
        /// </summary>
        /// <param name="sourceFileName">原文件路径。</param>
        /// <param name="destFileName">目的文件路径。</param>
        public static void MoveFile(string sourceFileName, string destFileName)
        {
            EnsureFile(destFileName);
            DeleteFile(destFileName);
            File.Move(sourceFileName, destFileName);
        }

        /// <summary>
        /// 获取指定文件的扩展名。
        /// </summary>
        /// <param name="fileName">文件路径。</param>
        /// <returns>文件的扩展名。</returns>
        public static string GetFileExtName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            var index = fileName.LastIndexOf('.');
            return fileName.Substring(index).ToLower();
        }
        #endregion

        #region IP
        /// <summary>
        /// 获取指定 IP 地址的中文名称。
        /// </summary>
        /// <param name="ipAddress">IP 地址。</param>
        /// <returns>IP 地址中文名称。</returns>
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

            private static readonly Hashtable cached = new Hashtable();

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

