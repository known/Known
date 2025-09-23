namespace Known.Models;

/// <summary>
/// 系统数据信息类。
/// </summary>
public class SystemDataInfo
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    public SystemDataInfo()
    {
        Version = Config.Version;
    }

    /// <summary>
    /// 取得或设置系统信息对象。
    /// </summary>
    public SystemInfo System { get; set; }

    /// <summary>
    /// 取得或设置系统运行时长。
    /// </summary>
    public double RunTime { get; set; }

    /// <summary>
    /// 取得或设置系统版本信息对象。
    /// </summary>
    public VersionInfo Version { get; set; }
}