using System.Runtime.InteropServices;

namespace Known.Helpers;

/// <summary>
/// 硬件信息帮助者类。
/// </summary>
public class HardwareHelper
{
    /// <summary>
    /// 获取 CPU 处理器序列号。
    /// </summary>
    /// <returns></returns>
    public static string GetProcessorId()
    {
        // Windows: 通过 WMIC 获取 ProcessorId
        // Linux:   通过 dmidecode 获取 CPU ID (需要 root 权限)
        // macOS:   通过 sysctl 获取 CPU 信息（无直接序列号，需用其他字段）
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var output = Processer.Execute("wmic", "cpu get ProcessorId");
            return output.Split('\n')[1].Trim();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var output = Processer.Execute("dmidecode", "-t processor");
            return output.Split(["ID: "], StringSplitOptions.None)[1].Split('\n')[0].Trim();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var output = Processer.Execute("sysctl", "-n machdep.cpu.vendor machdep.cpu.brand_string", false);
            return output.Trim().Replace("\n", " ");
        }
        return "Unsupported Platform";
    }

    /// <summary>
    /// 获取内存序列号。
    /// </summary>
    public static string GetMemorySerialNumber()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var output = Processer.Execute("wmic", "memorychip get SerialNumber");
            return output.Split('\n')[1].Trim();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var output = Processer.Execute("sudo", "dmidecode -t memory");
            return output.Split(["Serial Number:"], StringSplitOptions.None)[1].Split('\n')[0].Trim();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var output = Processer.Execute("system_profiler", "SPMemoryDataType");
            return output.Split(["Serial Number:"], StringSplitOptions.None)[1].Split('\n')[0].Trim();
        }
        return "Unsupported Platform";
    }

    /// <summary>
    /// 获取硬盘序列号。
    /// </summary>
    /// <returns></returns>
    public static string GetDiskSerialNumber()
    {
        // Windows: 通过 WMIC 获取磁盘序列号
        // Linux:   通过 udev 信息获取（如 /dev/sda）
        // macOS:   通过 diskutil 获取序列号
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var output = Processer.Execute("wmic", "diskdrive get SerialNumber");
            return output.Split('\n')[1].Trim();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var output = Processer.Execute("udevadm", "info --query=property --name=/dev/sda");
            return output.Split(["ID_SERIAL_SHORT="], StringSplitOptions.None)[1].Split('\n')[0].Trim();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var output = Processer.Execute("diskutil", "info disk0", false);
            return output.Split(["Device / Media Serial Number:"], StringSplitOptions.None)[1].Split('\n')[0].Trim();
        }
        return "Unsupported Platform";
    }
}