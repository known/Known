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
    /// 取得或设置表格是否可调整大小。
    /// </summary>
    public bool Resizable { get; set; }

    /// <summary>
    /// 取得或设置表格是否自动适应高度，默认是。
    /// </summary>
    public bool AutoHeight { get; set; } = true;

    /// <summary>
    /// 取得或设置表格是否显示自动序号。
    /// </summary>
    public bool ShowIndex { get; set; }

    /// <summary>
    /// 取得或设置表格是否显示分页。
    /// </summary>
    public bool ShowPager { get; set; }

    /// <summary>
    /// 取得或设置表格分页每页大小。
    /// </summary>
    public int? PageSize { get; set; }

    /// <summary>
    /// 取得或设置表格选择列选择框类型。
    /// </summary>
    public TableSelectType SelectType { get; set; }

    /// <summary>
    /// 取得或设置表格固定高度。
    /// </summary>
    public string FixedHeight { get; set; }

    /// <summary>
    /// 取得或设置表格操作列宽度，默认140。
    /// </summary>
    public string ActionWidth { get; set; } = "140";

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
    /// 取得表格栏位呈现模板字典。
    /// </summary>
    internal Dictionary<string, RenderFragment<TItem>> Templates { get; } = [];
}