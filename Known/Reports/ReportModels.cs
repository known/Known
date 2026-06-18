namespace Known.Reports;

public class ReportInfo
{
    public ReportInfo()
    {
        Id = Utils.GetNextId();
        CreateTime = DateTime.Now;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; } = "Table";
    public string Config { get; set; }
    public string Note { get; set; }
    public DateTime CreateTime { get; set; }
}

public class ReportConfig
{
    public string DataSource { get; set; }
    public ChartConfig Chart { get; set; }
    public TableConfig Table { get; set; }
}

public class ChartConfig
{
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

public enum ReportDisplayType
{
    Table,
    Chart,
    Combination
}
