namespace Known.Reports;

/// <summary>
/// 报表定义实体类。
/// </summary>
[DisplayName("报表定义")]
public class SysReport : EntityBase
{
    /// <summary>
    /// 取得或设置报表编码。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(Width = 120, IsQuery = true)]
    [DisplayName("代码")]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置报表名称。
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column(Width = 180, IsQuery = true)]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置业务类型。
    /// </summary>
    [MaxLength(50)]
    [Column(Width = 120, IsQuery = true)]
    [DisplayName("业务类型")]
    public string BizType { get; set; }

    /// <summary>
    /// 取得或设置分组。
    /// </summary>
    [MaxLength(50)]
    [Column(Width = 120, IsQuery = true)]
    [DisplayName("分组")]
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置图标。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("图标")]
    public string Icon { get; set; } = "bar-chart";

    /// <summary>
    /// 取得或设置数据源类型。
    /// </summary>
    [Column(Width = 100)]
    [DisplayName("数据源类型")]
    public string SourceType { get; set; } = nameof(ReportSourceType.Table);

    /// <summary>
    /// 取得或设置数据源。
    /// Table类型为表名，Sql类型为完整查询SQL。
    /// </summary>
    [DisplayName("数据源")]
    public string Source { get; set; }

    /// <summary>
    /// 取得或设置排序。
    /// </summary>
    [Column(Width = 80)]
    [DisplayName("顺序")]
    public int? Sort { get; set; }

    /// <summary>
    /// 取得或设置是否启用分页。
    /// </summary>
    [DisplayName("分页")]
    public bool IsPaging { get; set; } = true;

    /// <summary>
    /// 取得或设置报表说明。
    /// </summary>
    [DisplayName("说明")]
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [DisplayName("备注")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置报表视图类型。
    /// </summary>
    [DisplayName("视图类型")]
    public string ViewType { get; set; } = nameof(ReportViewType.Table);

    /// <summary>
    /// 取得或设置图表类型。
    /// </summary>
    [DisplayName("图表类型")]
    public string ChartType { get; set; } = nameof(ReportChartType.Bar);

    /// <summary>
    /// 取得或设置图表分类字段。
    /// </summary>
    [DisplayName("图表分类字段")]
    public string ChartCategoryField { get; set; }

    /// <summary>
    /// 取得或设置图表数值字段。
    /// </summary>
    [DisplayName("图表数值字段")]
    public string ChartValueField { get; set; }

    /// <summary>
    /// 取得或设置图表系列字段。
    /// </summary>
    [DisplayName("图表系列字段")]
    public string ChartSeriesField { get; set; }

    /// <summary>
    /// 取得或设置图表高度。
    /// </summary>
    [DisplayName("图表高度")]
    public int? ChartHeight { get; set; } = 360;

    /// <summary>
    /// 取得或设置报表字段集合。
    /// </summary>
    [DisplayName("字段")]
    public List<ReportFieldInfo> Fields { get; set; } = [];
}

/// <summary>
/// 报表数据源类型枚举。
/// </summary>
public enum ReportSourceType
{
    /// <summary>
    /// 数据表。
    /// </summary>
    Table,
    /// <summary>
    /// 自定义SQL。
    /// </summary>
    Sql
}

/// <summary>
/// 报表视图类型枚举。
/// </summary>
public enum ReportViewType
{
    Table,
    Chart,
    Both
}

/// <summary>
/// 报表图表类型枚举。
/// </summary>
public enum ReportChartType
{
    Bar,
    Line,
    Pie
}

/// <summary>
/// 报表字段配置类。
/// </summary>
public class ReportFieldInfo
{
    /// <summary>
    /// 取得或设置字段ID。
    /// </summary>
    [Required]
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置字段名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置字段表达式。
    /// </summary>
    public string Expression { get; set; }

    /// <summary>
    /// 取得或设置字段类型。
    /// </summary>
    public FieldType Type { get; set; } = FieldType.Text;

    /// <summary>
    /// 取得或设置代码表分类。
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置显示宽度。
    /// </summary>
    public int? Width { get; set; } = 140;

    /// <summary>
    /// 取得或设置对齐方式。
    /// </summary>
    public string Align { get; set; }

    /// <summary>
    /// 取得或设置是否允许查询。
    /// </summary>
    public bool IsQuery { get; set; }

    /// <summary>
    /// 取得或设置是否显示。
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// 取得或设置是否可排序。
    /// </summary>
    public bool IsSort { get; set; } = true;

    /// <summary>
    /// 取得或设置默认排序。
    /// </summary>
    public string DefaultSort { get; set; }

    /// <summary>
    /// 取得或设置单位。
    /// </summary>
    public string Unit { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    public string Note { get; set; }

    internal ColumnInfo ToColumn()
    {
        return new ColumnInfo
        {
            Id = Id,
            Name = Name,
            Type = Type,
            Category = Category,
            Width = Width,
            Align = Align,
            IsQuery = IsQuery,
            IsVisible = IsVisible,
            IsSort = IsSort,
            DefaultSort = DefaultSort,
            Unit = Unit,
            Note = Note,
            Ellipsis = true
        };
    }

    internal static ReportFieldInfo FromField(FieldInfo field)
    {
        if (field == null)
            return null;

        return new ReportFieldInfo
        {
            Id = field.Id,
            Name = field.Name,
            Type = field.Type,
            Width = field.Type == FieldType.TextArea ? 220 : 140,
            IsVisible = true,
            IsSort = true
        };
    }
}

/// <summary>
/// 报表预览结果类。
/// </summary>
public class ReportPreviewInfo
{
    /// <summary>
    /// 取得或设置预览SQL。
    /// </summary>
    public string Sql { get; set; }

    /// <summary>
    /// 取得或设置字段集合。
    /// </summary>
    public List<ReportFieldInfo> Fields { get; set; } = [];

    /// <summary>
    /// 取得或设置预览数据。
    /// </summary>
    public List<Dictionary<string, object>> Rows { get; set; } = [];
}