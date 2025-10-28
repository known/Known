namespace Known;

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
    /// 取得或设置统计数量1。
    /// </summary>
    public int Count1 { get; set; }

    /// <summary>
    /// 取得或设置统计数量2。
    /// </summary>
    public int Count2 { get; set; }

    /// <summary>
    /// 取得或设置统计数量3。
    /// </summary>
    public int Count3 { get; set; }

    /// <summary>
    /// 取得或设置统计数量。
    /// </summary>
    public int TotalCount { get; set; }
}

/// <summary>
/// 统计数量信息类。
/// </summary>
public class StatisticCountInfo
{
    /// <summary>
    /// 取得或设置统计类型（总、年、月等）。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置统计名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置统计数量。
    /// </summary>
    public int? Count { get; set; }

    /// <summary>
    /// 取得或设置点击数量导航的地址。
    /// </summary>
    public string Url { get; set; }
}

/// <summary>
/// 附件数据信息类。
/// </summary>
public class FileDataInfo
{
    /// <summary>
    /// 构造函数，创建一个附件数据信息类实例。
    /// </summary>
    public FileDataInfo() { }

    /// <summary>
    /// 构造函数，创建一个附件数据信息类实例。
    /// </summary>
    /// <param name="name">附件名称。</param>
    /// <param name="bytes">附件数据。</param>
    public FileDataInfo(string name, byte[] bytes)
    {
        Name = name;
        Size = bytes == null ? 0 : bytes.Length;
        Bytes = bytes;
    }

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

    /// <summary>
    /// 取得或设置缩略图字节数组。
    /// </summary>
    public byte[] Thumbnails { get; set; }

    internal int? MaxSize { get; set; }
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
/// 通知信息类。
/// </summary>
public class NotifyInfo
{
    /// <summary>
    /// 取得或设置通知窗口显示类型。
    /// </summary>
    public StyleType Type { get; set; }

    /// <summary>
    /// 取得或设置通知窗口标题。
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 取得或设置通知窗口内容。
    /// </summary>
    public string Message { get; set; }
}