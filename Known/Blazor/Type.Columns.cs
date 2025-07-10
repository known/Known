using AntDesign;

namespace Known.Blazor;

/// <summary>
/// 字典类型列组件类。
/// </summary>
public class DictionaryColumn : PropertyColumn<Dictionary<string, object>, object>
{
    [CascadingParameter] private UIContext UIContext { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        Title = UIContext.Language[Title];
        base.OnParametersSet();
    }
}

/// <summary>
/// 字符串类型列组件类。
/// </summary>
public class StringColumn : Column<string>
{
    [CascadingParameter] private UIContext UIContext { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        Title = UIContext.Language[Title];
        base.OnParametersSet();
    }
}

/// <summary>
/// 布尔类型列组件类。
/// </summary>
public class BooleanColumn : Column<bool>
{
    [CascadingParameter] private UIContext UIContext { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        Title = UIContext.Language[Title];
        base.OnParametersSet();
    }
}

/// <summary>
/// 整数类型列组件类。
/// </summary>
public class IntegerColumn : Column<int?>
{
    [CascadingParameter] private UIContext UIContext { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        Title = UIContext.Language[Title];
        base.OnParametersSet();
    }
}

/// <summary>
/// 小数类型列组件类。
/// </summary>
public class DecimalColumn : Column<decimal?>
{
    [CascadingParameter] private UIContext UIContext { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        Title = UIContext.Language[Title];
        base.OnParametersSet();
    }
}

/// <summary>
/// 日期类型列组件类。
/// </summary>
public class DateColumn : Column<DateTime?>
{
    [CascadingParameter] private UIContext UIContext { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Format = Config.DateFormat;
        Width = "120";
        Align = ColumnAlign.Center;
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        Title = UIContext.Language[Title];
        base.OnParametersSet();
    }
}

/// <summary>
/// 日期时间类型列组件类。
/// </summary>
public class DateTimeColumn : Column<DateTime?>
{
    [CascadingParameter] private UIContext UIContext { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Format = Config.DateTimeFormat;
        Width = "150";
        Align = ColumnAlign.Center;
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        Title = UIContext.Language[Title];
        base.OnParametersSet();
    }
}

/// <summary>
/// 表格列组件类。
/// </summary>
public class TableColumn : Column<string>
{
    [CascadingParameter] private IComContainer AntTable { get; set; }
    [CascadingParameter] private UIContext UIContext { get; set; }

    /// <summary>
    /// 取得或设置显示数据。
    /// </summary>
    [Parameter] public object Value { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (AntTable != null && AntTable.IsView)
            ChildContent = b => b.Text(Value?.ToString());
        Title = UIContext.Language[Title];
        base.OnParametersSet();
    }
}