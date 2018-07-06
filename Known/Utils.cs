using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Known
{
    public class Utils
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

        public static string HideMobile(string mobile)
        {
            return Regex.Replace(mobile, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
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
