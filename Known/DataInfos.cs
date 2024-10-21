namespace Known;

/// <summary>
/// 后台管理主页数据交互信息类。
/// </summary>
public class AdminInfo
{
    /// <summary>
    /// 取得或设置系统名称。
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// 取得或设置当前用户未读消息数量（暂未使用）。
    /// </summary>
    public int MessageCount { get; set; }

    /// <summary>
    /// 取得或设置当前用户系统设置信息，如：是否使用多标签页模式。
    /// </summary>
    public SettingInfo UserSetting { get; set; }

    /// <summary>
    /// 取得或设置当前用户模块表格设置信息列表，如：设置模块表格的显隐和宽度。
    /// </summary>
    public Dictionary<string, List<TableSettingInfo>> UserTableSettings { get; set; }

    /// <summary>
    /// 取得或设置当前用户权限菜单列表。
    /// </summary>
    public List<MenuInfo> UserMenus { get; set; }

    /// <summary>
    /// 取得或设置系统数据字典和代码表信息列表，用于前后端分离时，缓存在前端。
    /// </summary>
    public List<CodeInfo> Codes { get; set; }
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
/// 数据数量统计信息类。
/// </summary>
public class CountInfo
{
    /// <summary>
    /// 取得或设置统计字段1。
    /// </summary>
    public string Field1 { get; set; }

    /// <summary>
    /// 取得或设置统计字段2。
    /// </summary>
    public string Field2 { get; set; }

    /// <summary>
    /// 取得或设置统计字段3。
    /// </summary>
    public string Field3 { get; set; }

    /// <summary>
    /// 取得或设置统计数量。
    /// </summary>
    public int TotalCount { get; set; }
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

/// <summary>
/// 定时任务摘要信息类。
/// </summary>
public class TaskSummaryInfo
{
    /// <summary>
    /// 取得或设置定时任务当前状态。
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// 取得或设置定时任务当前描述信息。
    /// </summary>
    public string Message { get; set; }
}