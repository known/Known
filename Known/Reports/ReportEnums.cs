namespace Known.Reports;

/// <summary>
/// 报表类型枚举。
/// </summary>
public enum ReportBlockType
{
    /// <summary>
    /// 表格。
    /// </summary>
    [Display(Name = "表格")]
    Table,
    /// <summary>
    /// 图表。
    /// </summary>
    [Display(Name = "图表")]
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
    [Display(Name = "示例数据")]
    Sample,
    /// <summary>
    /// SQL语句。
    /// </summary>
    [Display(Name = "SQL语句")]
    SQL,
    /// <summary>
    /// API接口。
    /// </summary>
    [Display(Name = "API接口")]
    Api,
    /// <summary>
    /// 系统实体。
    /// </summary>
    [Display(Name = "系统实体")]
    Entity
}

/// <summary>
/// 聚合函数类型枚举。
/// </summary>
public enum AggregateType
{
    /// <summary>
    /// 无聚合。
    /// </summary>
    [Display(Name = "无")]
    None,
    /// <summary>
    /// 计数。
    /// </summary>
    [Display(Name = "计数(Count)")]
    Count,
    /// <summary>
    /// 求和。
    /// </summary>
    [Display(Name = "求和(Sum)")]
    Sum,
    /// <summary>
    /// 平均值。
    /// </summary>
    [Display(Name = "平均值(Avg)")]
    Avg,
    /// <summary>
    /// 最大值。
    /// </summary>
    [Display(Name = "最大值(Max)")]
    Max,
    /// <summary>
    /// 最小值。
    /// </summary>
    [Display(Name = "最小值(Min)")]
    Min
}

/// <summary>
/// 图表类型枚举。
/// </summary>
public enum ChartType
{
    /// <summary>
    /// 柱状图。
    /// </summary>
    [Display(Name = "柱状图")]
    Bar,
    /// <summary>
    /// 折线图。
    /// </summary>
    [Display(Name = "折线图")]
    Line,
    /// <summary>
    /// 饼图。
    /// </summary>
    [Display(Name = "饼图")]
    Pie,
    /// <summary>
    /// 面积图。
    /// </summary>
    [Display(Name = "面积图")]
    Area,
    /// <summary>
    /// 散点图。
    /// </summary>
    [Display(Name = "散点图")]
    Scatter
}