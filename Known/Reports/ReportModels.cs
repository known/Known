namespace Known.Reports;

/// <summary>
/// 报表类型枚举。
/// </summary>
public enum ReportType
{
    /// <summary>
    /// 表格。
    /// </summary>
    Table,
    /// <summary>
    /// 图表。
    /// </summary>
    Chart,
    /// <summary>
    /// 组合。
    /// </summary>
    Combination
}

/// <summary>
/// 报表配置类。
/// </summary>
public class ReportConfig
{
    /// <summary>
    /// 取得或设置图表配置列表，旧版兼容。
    /// </summary>
    public List<ChartConfig> Charts { get; set; }
    /// <summary>
    /// 取得或设置表格配置列表，旧版兼容。
    /// </summary>
    public List<TableConfig> Tables { get; set; }
    /// <summary>
    /// 取得或设置报表块列表。
    /// </summary>
    public List<ReportBlock> Blocks { get; set; }
}

/// <summary>
/// 数据源配置类。
/// </summary>
public class DataSourceConfig
{
    /// <summary>
    /// 取得或设置数据源类型（Sample、Table、SQL、Api）。
    /// </summary>
    public string SourceType { get; set; } = "Sample";
    /// <summary>
    /// 取得或设置数据库连接ID。
    /// </summary>
    public string ConnectionId { get; set; }
    /// <summary>
    /// 取得或设置数据源名称（表名、SQL语句、API地址）。
    /// </summary>
    public string SourceName { get; set; }
    /// <summary>
    /// 取得或设置API请求方式。
    /// </summary>
    public string RequestMethod { get; set; }
    /// <summary>
    /// 取得或设置API请求地址。
    /// </summary>
    public string RequestUrl { get; set; }
}

/// <summary>
/// 报表块配置类。
/// </summary>
public class ReportBlock
{
    /// <summary>
    /// 取得或设置块ID。
    /// </summary>
    public string Id { get; set; } = Utils.GetGuid();
    /// <summary>
    /// 取得或设置块类型（Chart、Table）。
    /// </summary>
    public string BlockType { get; set; } = "Chart";
    /// <summary>
    /// 取得或设置行号。
    /// </summary>
    public int Row { get; set; }
    /// <summary>
    /// 取得或设置列号。
    /// </summary>
    public int Col { get; set; }
    /// <summary>
    /// 取得或设置宽度（栅格列数）。
    /// </summary>
    public int? Width { get; set; } = 12;
    /// <summary>
    /// 取得或设置高度。
    /// </summary>
    public int Height { get; set; } = 1;
    /// <summary>
    /// 取得或设置标题。
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// 取得或设置数据源配置。
    /// </summary>
    public DataSourceConfig DataSource { get; set; }
    /// <summary>
    /// 取得或设置图表配置。
    /// </summary>
    public ChartConfig Chart { get; set; }
    /// <summary>
    /// 取得或设置表格配置。
    /// </summary>
    public TableConfig Table { get; set; }
}

/// <summary>
/// 图表配置类。
/// </summary>
public class ChartConfig
{
    /// <summary>
    /// 取得或设置数据源配置。
    /// </summary>
    public DataSourceConfig DataSource { get; set; }
    /// <summary>
    /// 取得或设置图表类型。
    /// </summary>
    public string ChartType { get; set; } = "bar";
    /// <summary>
    /// 取得或设置图表标题。
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// 取得或设置图表宽度。
    /// </summary>
    public int? Width { get; set; } = 600;
    /// <summary>
    /// 取得或设置图表高度。
    /// </summary>
    public int? Height { get; set; } = 400;
    /// <summary>
    /// 取得或设置X轴字段。
    /// </summary>
    public string XField { get; set; }
    /// <summary>
    /// 取得或设置Y轴字段。
    /// </summary>
    public string YField { get; set; }
    /// <summary>
    /// 取得或设置分组字段。
    /// </summary>
    public string CategoryField { get; set; }
}

/// <summary>
/// 表格配置类。
/// </summary>
public class TableConfig
{
    /// <summary>
    /// 取得或设置数据源配置。
    /// </summary>
    public DataSourceConfig DataSource { get; set; }
    /// <summary>
    /// 取得或设置列定义列表。
    /// </summary>
    public List<ColumnConfig> Columns { get; set; } = [];
}

/// <summary>
/// 列配置类。
/// </summary>
public class ColumnConfig
{
    /// <summary>
    /// 取得或设置字段名。
    /// </summary>
    public string Field { get; set; }
    /// <summary>
    /// 取得或设置标题。
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// 取得或设置宽度。
    /// </summary>
    public int? Width { get; set; } = 100;
    /// <summary>
    /// 取得或设置对齐方式。
    /// </summary>
    public string Align { get; set; } = "left";
    /// <summary>
    /// 取得或设置类型（text、number、date）。
    /// </summary>
    public string Type { get; set; } = "text";
}
