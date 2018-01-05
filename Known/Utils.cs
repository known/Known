using System;
using System.IO;

namespace Known
{
    /// <summary>
    /// 效用类。
    /// </summary>
    public class Utils
    {
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

        #region File
        /// <summary>
        /// 确信文件路径存在，若不存在，则自动创建。
        /// </summary>
        /// <param name="fileName">文件路径。</param>
        /// <returns>安全可用的文件路径。</returns>
        public static string EnsureFile(string fileName)
        {
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
