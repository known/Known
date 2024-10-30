using System.Net.NetworkInformation;
using System.Security.Cryptography;

namespace Known.Core;

/// <summary>
/// 后端效用类。
/// </summary>
public class CoreUtils
{
    #region Encryptor
    /// <summary>
    /// 获取字符串的MD5加密字符串。
    /// </summary>
    /// <param name="value">原字符串。</param>
    /// <returns>MD5加密字符串。</returns>
    public static string ToMd5(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var buffer = Encoding.UTF8.GetBytes(value);
        var bytes = MD5.HashData(buffer);

        var sb = new StringBuilder();
        foreach (var item in bytes)
        {
            sb.Append(item.ToString("x2"));
        }
        return sb.ToString();
    }
    #endregion

    #region Network
    /// <summary>
    /// 用Ping检查主机IP或域名是否连接。
    /// </summary>
    /// <param name="host">主机IP或域名。</param>
    /// <param name="timeout">ping超时时间。</param>
    /// <returns>联网返回True，否则False。</returns>
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

    /// <summary>
    /// 判断主机是否已经连接网络。
    /// </summary>
    /// <returns></returns>
    public static bool HasNetwork() => Ping("www.baidu.com");
    #endregion
}