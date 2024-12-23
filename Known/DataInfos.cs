namespace Known;

/// <summary>
/// 框架配置数据信息类。
/// </summary>
public partial class AppDataInfo
{
    /// <summary>
    /// 取得或设置顶部导航信息列表。
    /// </summary>
    public List<TopNavInfo> TopNavs { get; set; }

    /// <summary>
    /// 取得或设置模块信息列表。
    /// </summary>
    public List<ModuleInfo> Modules { get; set; }
}

/// <summary>
/// 顶部导航信息类。
/// </summary>
public class TopNavInfo
{
    /// <summary>
    /// 取得或设置自定义导航项类型。
    /// </summary>
    public string NavItemType { get; set; }
}

/// <summary>
/// 系统配置数据交互信息类。
/// </summary>
public class ConfigInfo
{
    /// <summary>
    /// 取得或设置配置数据键。
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 取得或设置配置数据对象。
    /// </summary>
    public object Value { get; set; }
}

/// <summary>
/// 自动页面数据交互信息类。
/// </summary>
/// <typeparam name="TData">交互数据泛型类型。</typeparam>
public class AutoInfo<TData>
{
    /// <summary>
    /// 取得或设置页面ID。
    /// </summary>
    public string PageId { get; set; }

    /// <summary>
    /// 取得或设置交互数据泛型对象。
    /// </summary>
    public TData Data { get; set; }
}

/// <summary>
/// 图表数据信息类。
/// </summary>
public class ChartDataInfo
{
    /// <summary>
    /// 取得或设置图表名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置图表数据字典。
    /// </summary>
    public Dictionary<string, object> Series { get; set; }
}

/// <summary>
/// 附件数据信息类。
/// </summary>
public class FileDataInfo
{
    /// <summary>
    /// 取得或设置附件文件名。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置附件文件大小。
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// 取得或设置附件内容字节数组。
    /// </summary>
    public byte[] Bytes { get; set; }
}

/// <summary>
/// 附件URL地址信息类。
/// </summary>
public class FileUrlInfo
{
    /// <summary>
    /// 取得或设置附件文件名。
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 取得或设置附件图片缩略图URL。
    /// </summary>
    public string ThumbnailUrl { get; set; }

    /// <summary>
    /// 取得或设置附件原始文件URL。
    /// </summary>
    public string OriginalUrl { get; set; }
}