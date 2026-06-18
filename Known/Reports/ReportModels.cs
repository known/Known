namespace Known.Reports;

public enum ReportType
{
    Table,
    Chart,
    Combination
}

public class ReportConfig
{
    public List<ChartConfig> Charts { get; set; }
    public List<TableConfig> Tables { get; set; }
}

public class DataSourceConfig
{
    public string DataSource { get; set; }
}

public class ChartConfig
{
    public DataSourceConfig DataSource { get; set; }
    public string ChartType { get; set; } = "bar";
    public string Title { get; set; }
    public int? Width { get; set; } = 600;
    public int? Height { get; set; } = 400;
    public string XField { get; set; }
    public string YField { get; set; }
    public string CategoryField { get; set; }
}

public class TableConfig
{
    public DataSourceConfig DataSource { get; set; }
    public List<ColumnConfig> Columns { get; set; } = [];
}

public class ColumnConfig
{
    public string Field { get; set; }
    public string Title { get; set; }
    public int? Width { get; set; } = 100;
    public string Align { get; set; } = "left";
    public string Type { get; set; } = "text";
}