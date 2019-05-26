using System;
using System.Security.Cryptography;
using System.Text;

namespace Known
{
    /// <summary>
    /// 加解密操作类。
    /// </summary>
    public sealed class Encryptor
    {
        /// <summary>
        /// 将字符串进行 MD5 加密。
        /// </summary>
        /// <param name="value">原字符串。</param>
        /// <returns>加密字符串。</returns>
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

        /// <summary>
        /// 将字符串进行 DES 加密。
        /// </summary>
        /// <param name="value">原字符串。</param>
        /// <param name="password">加密密码，默认空。</param>
        /// <returns>加密字符串。</returns>
        public static string DESEncrypt(string value, string password = "")
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var buffer = Encoding.UTF8.GetBytes(password);
            var key = new MD5CryptoServiceProvider().ComputeHash(buffer);
            var des = new TripleDESCryptoServiceProvider
            {
                Key = key,
                Mode = CipherMode.ECB
            };
            var datas = Encoding.UTF8.GetBytes(value);
            var bytes = des.CreateEncryptor()
                           .TransformFinalBlock(datas, 0, datas.Length);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 将 DES 加密的字符串进行揭秘。
        /// </summary>
        /// <param name="value">加密字符串。</param>
        /// <param name="password">揭秘密码，默认空。</param>
        /// <returns>解密字符串。</returns>
        public static string DESDecrypt(string value, string password = "")
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var buffer = Encoding.UTF8.GetBytes(password);
            var key = new MD5CryptoServiceProvider().ComputeHash(buffer);
            var des = new TripleDESCryptoServiceProvider
            {
                Key = key,
                Mode = CipherMode.ECB
            };
            var datas = Convert.FromBase64String(value);
            var bytes = des.CreateDecryptor()
                           .TransformFinalBlock(datas, 0, datas.Length);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
