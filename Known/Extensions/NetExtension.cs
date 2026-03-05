using System.Net;
using System.Net.Sockets;

namespace Known.Extensions;

/// <summary>
/// 网络扩展类。
/// </summary>
public static class NetExtension
{
    /// <summary>
    /// 判断访问IP是否是内网访问，需要配置转发头，防止获取的是代理服务器IP。
    /// </summary>
    /// <param name="ipAddress">客户端IP地址。</param>
    /// <returns></returns>
    public static bool IsInternal(this IPAddress ipAddress)
    {
        if (ipAddress == null)
            return false;

        // 处理 IPv6 链路本地地址 (fe80::/10)
        if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
        {
            // IPv6 链路本地地址范围: FE80:: 到 FEBF::
            // 通过检查前10位来判断
            byte[] addressBytes = ipAddress.GetAddressBytes();
            return addressBytes[0] == 0xFE && (addressBytes[1] & 0xC0) == 0x80;
        }

        // 处理 IPv4 私有地址
        if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
        {
            byte[] bytes = ipAddress.GetAddressBytes();
            // 10.0.0.0/8
            if (bytes[0] == 10)
                return true;
            // 172.16.0.0/12
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
                return true;
            // 192.168.0.0/16
            if (bytes[0] == 192 && bytes[1] == 168)
                return true;
            // 回环地址 127.0.0.1 通常也被视为本地/内部
            if (bytes[0] == 127)
                return true;
        }

        return false;
    }
}