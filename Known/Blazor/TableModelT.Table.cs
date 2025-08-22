namespace Known.Blazor;

partial class TableModel<TItem>
{
    /// <summary>
    /// 取得表格操作列是否有操作按钮。
    /// </summary>
    public bool HasAction => Actions != null && Actions.Count > 0;

    /// <summary>
    /// 取得表格是否有汇总字段列。
    /// </summary>
    public bool HasSum => Columns != null && Columns.Any(c => c.IsSum);

    /// <summary>
    /// 取得或设置表格行是否显示斑马纹，默认显示。
    /// </summary>
    public bool IsStriped { get; set; } = true;

    /// <summary>
    /// 取得或设置表格行列是否支持滚动，默认支持。
    /// </summary>
    public bool IsScroll { get; set; } = true;

    /// <summary>
    /// 取得或设置表格列是否可调整列宽度大小，默认可调整。
    /// </summary>
    public bool Resizable { get; set; } = true;

    /// <summary>
    /// 取得或设置表格是否自动适应高度，默认是。
    /// </summary>
    public bool AutoHeight { get; set; } = true;

    /// <summary>
    /// 取得或设置表格是否显示单元格边框。
    /// </summary>
    public bool Bordered { get; set; }

    /// <summary>
    /// 取得或设置表格是否启用加载图标。
    /// </summary>
    public bool EnableSpin { get; set; }

    /// <summary>
    /// 取得或设置表格是否启用过滤，默认启用。
    /// </summary>
    public bool EnableFilter { get; set; } = true;

    /// <summary>
    /// 取得或设置表格是否启用排序，默认启用。
    /// </summary>
    public bool EnableSort { get; set; } = true;

    /// <summary>
    /// 取得或设置表格是否显示自动序号。
    /// </summary>
    public bool ShowIndex { get; set; }

    private bool showPager = true;
    /// <summary>
    /// 取得或设置表格是否显示分页，默认分页。
    /// </summary>
    public bool ShowPager
    {
        get => showPager;
        set
        {
            showPager = value;
            Criteria.IsPaging = value;
        }
    }

    private int? pageSize = Config.App.DefaultPageSize;
    /// <summary>
    /// 取得或设置表格分页每页大小。
    /// </summary>
    public int? PageSize
    {
        get => pageSize;
        set
        {
            pageSize = value;
            Criteria.PageSize = value ?? Config.App.DefaultPageSize;
        }
    }

    /// <summary>
    /// 取得或设置表格选择列选择框类型。
    /// </summary>
    public TableSelectType SelectType { get; set; }

    /// <summary>
    /// 取得或设置表格固定高度。
    /// </summary>
    public string FixedHeight { get; set; }

    /// <summary>
    /// 取得或设置表格操作列宽度，默认120。
    /// </summary>
    public string ActionWidth { get; set; } = "120";

    /// <summary>
    /// 取得或设置表格操作列显示按钮最大数量，默认2。
    /// </summary>
    public int ActionCount { get; set; } = 2;

    /// <summary>
    /// 取得或设置表格行数据主键委托。
    /// </summary>
    public Func<TItem, object> RowKey { get; set; }

    /// <summary>
    /// 取得或设置表格行CSS类名委托。
    /// </summary>
    public Func<TItem, string> RowClass { get; set; }

    /// <summary>
    /// 取得或设置表格行展开数据呈现模板。
    /// </summary>
    public Action<RenderTreeBuilder, TItem> ExpandTemplate { get; set; }

    /// <summary>
    /// 取得或设置表格行展开时回调，必须设置，否则展开按钮不显示。
    /// </summary>
    public EventCallback<RowData<TItem>> OnExpand { get; set; }

    /// <summary>
    /// 取得表格栏位呈现模板字典。
    /// </summary>
    internal Dictionary<string, RenderFragment<TItem>> Templates { get; } = [];
}