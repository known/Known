using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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
        #endregion

        #region Encrypt
        /// <summary>
        /// 将字符串加密成MD5格式。
        /// </summary>
        /// <param name="value">字符串。</param>
        /// <returns>加密字符串。</returns>
        public static string ToMd5(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            byte[] bytes;
            using (var md5 = MD5.Create())
            {
                bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
            }

            var sb = new StringBuilder();
            foreach (var item in bytes)
            {
                sb.Append(item.ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 以DES方式加密字符串。
        /// </summary>
        /// <param name="value">字符串。</param>
        /// <param name="password">加密密码。</param>
        /// <returns>加密字符串。</returns>
        public static string Encrypt(string value, string password = "")
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var key = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(password));
            var des = new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB };
            var datas = Encoding.UTF8.GetBytes(value);
            var bytes = des.CreateEncryptor().TransformFinalBlock(datas, 0, datas.Length);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 以DES方式解密字符串。
        /// </summary>
        /// <param name="value">加密字符串。</param>
        /// <param name="password">解密密码。</param>
        /// <returns>字符串。</returns>
        public static string Decrypt(string value, string password = "")
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var key = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(password));
            var des = new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB };
            var datas = Convert.FromBase64String(value);
            var bytes = des.CreateDecryptor().TransformFinalBlock(datas, 0, datas.Length);
            return Encoding.UTF8.GetString(bytes);
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
