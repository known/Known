using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Known
{
    /// <summary>
    /// 效用类。
    /// </summary>
    public class Utils
    {
        #region Base
        /// <summary>
        /// 取得新的GUID。
        /// </summary>
        public static string NewGuid
        {
            get { return Guid.NewGuid().ToString().ToLower().Replace("-", ""); }
        }

        /// <summary>
        /// 判断对象值是否为空或空字符串。
        /// </summary>
        /// <param name="value">对象值。</param>
        /// <returns>为空或空字符串返回true，否则返回false。</returns>
        public static bool IsNullOrEmpty(object value)
        {
            return value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString());
        }

        /// <summary>
        /// 转换数据类型。
        /// </summary>
        /// <typeparam name="T">转换目标类型。</typeparam>
        /// <param name="value">转换值（字符，数值，布林，枚举）。</param>
        /// <param name="defaultValue">转换为空时的默认值。</param>
        /// <returns>转换后的值。</returns>
        public static T ConvertTo<T>(object value, T defaultValue = default(T))
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            var valueString = value.ToString();
            var type = typeof(T);
            if (type == typeof(string))
                return (T)Convert.ChangeType(valueString, type);

            valueString = valueString.Trim();
            if (valueString.Length == 0)
                return defaultValue;

            if (type.IsEnum)
                return (T)Enum.Parse(type, valueString, true);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);

            if (type == typeof(bool) || type == typeof(bool?))
                valueString = ",1,Y,YES,TRUE,".Contains(valueString.ToUpper()) ? "True" : "False";

            try
            {
                return (T)Convert.ChangeType(valueString, type);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 将金额数值转换成人民币大写格式。
        /// </summary>
        /// <param name="value">金额数值。</param>
        /// <returns>金额人民币大写格式。</returns>
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

        /// <summary>
        /// 隐藏手机号码中间4位。
        /// </summary>
        /// <param name="mobile">手机号码。</param>
        /// <returns>隐藏的手机号码。</returns>
        public static string HideMobile(string mobile)
        {
            return Regex.Replace(mobile, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
        }
        #endregion

        #region File
        /// <summary>
        /// 确定指定的文件是否存在！
        /// </summary>
        /// <param name="fileName">文件路径。</param>
        /// <returns>存在文件返回True，否则返回False。</returns>
        public static bool ExistsFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            return File.Exists(fileName);
        }

        /// <summary>
        /// 确信文件夹路径存在，若不存在，则自动创建。
        /// </summary>
        /// <param name="path">文件夹路径。</param>
        /// <returns>安全可用的文件夹路径。</returns>
        public static string EnsureDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        /// <summary>
        /// 确信文件路径存在，若不存在，则自动创建。
        /// </summary>
        /// <param name="fileName">文件路径。</param>
        /// <returns>安全可用的文件路径。</returns>
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
        /// 删除文件。
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
        /// 获取文件扩展名。
        /// </summary>
        /// <param name="fileName">文件路径。</param>
        /// <returns>文件扩展名。</returns>
        public static string GetFileExtName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            var index = fileName.LastIndexOf('.');
            return fileName.Substring(index).ToLower();
        }
        #endregion
    }
}
