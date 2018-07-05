using System;
using System.Security.Cryptography;
using System.Text;

namespace Known
{
    /// <summary>
    /// 加密器。
    /// </summary>
    public class Encryptor
    {
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
    }
}
