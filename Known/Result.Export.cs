namespace Known;

/// <summary>
/// 导出栏位信息类。
/// </summary>
public class ExportColumnInfo
{
    /// <summary>
    /// 取得或设置导出栏位ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置导出栏位名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置导出栏位代码表类别名。
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置导出栏位字段类型。
    /// </summary>
    public FieldType Type { get; set; }

    /// <summary>
    /// 取得或设置导出栏位是否是附加栏位。
    /// </summary>
    public bool IsAdditional { get; set; }
}

/// <summary>
/// 统计栏位信息类。
/// </summary>
public class StatisticColumnInfo
{
    /// <summary>
    /// 取得或设置统计栏位ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置统计栏位功能方法，默认为sum。
    /// </summary>
    public string Function { get; set; } = "sum";

    /// <summary>
    /// 取得或设置统计栏位SQL语句表达式。
    /// </summary>
    public string Expression { get; set; }
}