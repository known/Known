namespace Known.Reports;

/// <summary>
/// 报表类型枚举。
/// </summary>
public enum ReportBlockType
{
    /// <summary>
    /// 表格。
    /// </summary>
    [Description("表格")]
    Table,
    /// <summary>
    /// 图表。
    /// </summary>
    [Description("图表")]
    Chart
}

/// <summary>
/// 数据源类型枚举。
/// </summary>
public enum DataSourceType
{
    /// <summary>
    /// 示例数据。
    /// </summary>
    [Description("示例数据")]
    Sample,
    /// <summary>
    /// 数据库表。
    /// </summary>
    [Description("数据库表")]
    Table,
    /// <summary>
    /// SQL语句。
    /// </summary>
    [Description("SQL语句")]
    SQL,
    /// <summary>
    /// API接口。
    /// </summary>
    [Description("API接口")]
    Api
}

/// <summary>
/// 图表类型枚举。
/// </summary>
public enum ChartType
{
    /// <summary>
    /// 柱状图。
    /// </summary>
    [Description("柱状图")]
    Bar,
    /// <summary>
    /// 折线图。
    /// </summary>
    [Description("折线图")]
    Line,
    /// <summary>
    /// 饼图。
    /// </summary>
    [Description("饼图")]
    Pie,
    /// <summary>
    /// 面积图。
    /// </summary>
    [Description("面积图")]
    Area,
    /// <summary>
    /// 散点图。
    /// </summary>
    [Description("散点图")]
    Scatter
}