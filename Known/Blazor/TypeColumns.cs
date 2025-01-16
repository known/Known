using AntDesign;

namespace Known.Blazor;

/// <summary>
/// 字典类型列组件类。
/// </summary>
public class DictionaryColumn : PropertyColumn<Dictionary<string, object>, object>
{
    /// <summary>
    /// 取得或设置栏位配置信息。
    /// </summary>
    [Parameter] public ColumnInfo Column { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (Column != null)
        {
            Property = c => c.GetValue(Column.Id);
            Ellipsis = true;
            ShowSorterTooltip = false;
            if (!string.IsNullOrWhiteSpace(Column.Fixed))
                Fixed = Column.ToColumnFixPlacement();
            Sortable = Column.IsSort;
            DefaultSortOrder = Column.ToSortDirection();
            Align = Column.ToColumnAlign();
        }
    }
}

/// <summary>
/// 表格列扩展类。
/// </summary>
public class AntColumn : Column<object>
{
    /// <summary>
    /// 取得或设置栏位配置信息。
    /// </summary>
    [Parameter] public ColumnInfo Column { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (Column != null)
        {
            Ellipsis = true;
            ShowSorterTooltip = false;
            if (!string.IsNullOrWhiteSpace(Column.Fixed))
                Fixed = Column.ToColumnFixPlacement();
            Sortable = Column.IsSort;
            DefaultSortOrder = Column.ToSortDirection();
            Align = Column.ToColumnAlign();
        }
    }
}

/// <summary>
/// 字符串类型列组件类。
/// </summary>
public class StringColumn : Column<string> { }

/// <summary>
/// 布尔类型列组件类。
/// </summary>
public class BooleanColumn : Column<bool> { }

/// <summary>
/// 整数类型列组件类。
/// </summary>
public class IntegerColumn : Column<int?> { }

/// <summary>
/// 小数类型列组件类。
/// </summary>
public class DecimalColumn : Column<decimal?> { }

/// <summary>
/// 日期类型列组件类。
/// </summary>
public class DateColumn : Column<DateTime?>
{
    /// <summary>
    /// 构造函数，创建一个日期类型列组件类的实例。
    /// </summary>
    public DateColumn()
    {
        Format = Config.DateFormat;
        Width = "120";
        Align = ColumnAlign.Center;
    }
}

/// <summary>
/// 日期时间类型列组件类。
/// </summary>
public class DateTimeColumn : Column<DateTime?>
{
    /// <summary>
    /// 构造函数，创建一个日期时间类型列组件类的实例。
    /// </summary>
    public DateTimeColumn()
    {
        Format = Config.DateTimeFormat;
        Width = "150";
        Align = ColumnAlign.Center;
    }
}