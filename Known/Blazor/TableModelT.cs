namespace Known.Blazor;

/// <summary>
/// 泛型表格组件模型信息类。
/// </summary>
/// <typeparam name="TItem">表格行数据类型。</typeparam>
public partial class TableModel<TItem> : TableModel where TItem : class, new()
{
    private bool IsAuto { get; }

    /// <summary>
    /// 构造函数，创建一个泛型表格组件模型信息类的实例。
    /// </summary>
    /// <param name="page">表格关联的页面组件。</param>
    /// <param name="mode">根据数据类型自动生成表格列。</param>
    public TableModel(BaseComponent page, TableColumnMode mode = TableColumnMode.None) : base(page)
    {
        IsAuto = mode != TableColumnMode.None;
        AdvSearch = true;
        Page = page;
        Name = page?.Name;
        if (mode == TableColumnMode.Property)
            AllColumns = TypeHelper.Properties(typeof(TItem)).Select(p => new ColumnInfo(p)).ToList();
        else if (mode == TableColumnMode.Attribute)
            AllColumns = GetAttributeColumns(typeof(TItem));
        if (AllColumns != null && AllColumns.Count > 0)
            Columns = AllColumns;

        IsDictionary = typeof(TItem) == typeof(Dictionary<string, object>);
        OnAction = page.OnActionClick;
        Toolbar.OnItemClick = page.OnToolClick;
        var isPage = !IsAuto && page is BasePage;
        Initialize(isPage);
    }

    /// <summary>
    /// 取得表格用户列设置ID。
    /// </summary>
    public string SettingId => $"UserTable_{Context.Current?.Id}";

    /// <summary>
    /// 取得表格关联的页面组件。
    /// </summary>
    public BaseComponent Page { get; }

    /// <summary>
    /// 取得表格数据是否是字典类型。
    /// </summary>
    public bool IsDictionary { get; }

    /// <summary>
    /// 取得或设置表格是否显示工具条，默认显示。
    /// </summary>
    public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// 取得表格标签配置对象。
    /// </summary>
    public TabModel Tab { get; } = new TabModel { IsFillHeight = true };

    /// <summary>
    /// 取得或设置树形表格子节点表达式。
    /// </summary>
    public Func<TItem, List<TItem>> TreeChildren { get; set; }

    internal override string TableId => typeof(TItem).Name;
    internal override Type DataType => typeof(TItem);
    internal void Initialize() => Initialize(true);

    /// <summary>
    /// 初始化表格栏位、权限、查询条件。
    /// </summary>
    /// <param name="isPage">是否是表格页面。</param>
    protected virtual void Initialize(bool isPage)
    {
        if (isPage)
        {
            Clear();
            var menu = Context?.Current;
            if (menu != null)
            {
                Name = Language.GetString(menu);
                var param = menu.GetTablePageParameter();
                var model = DataHelper.ToEntity(param?.EntityData);
                SetPage(model, param?.Page, param?.Form);
            }
        }

        if (PageSize != null)
            Criteria.PageSize = PageSize.Value;

        SetQueryColumns();
    }

    /// <summary>
    /// 清理表格，恢复默认数据。
    /// </summary>
    public void Clear()
    {
        Columns?.Clear();
        QueryColumns?.Clear();
        QueryData?.Clear();
        Toolbar?.Items?.Clear();
        Actions?.Clear();
        Criteria?.Clear();
    }

    /// <summary>
    /// 设置无代码页面信息。
    /// </summary>
    /// <param name="model">实体模型。</param>
    /// <param name="info">页面模型。</param>
    /// <param name="form">表单模型。</param>
    public void SetPage(EntityInfo model, PageInfo info, FormInfo form)
    {
        if (info == null)
            return;

        //FixedWidth = info.FixedWidth;
        //FixedHeight = info.FixedHeight;
        ShowPager = info.ShowPager;
        if (info.PageSize != null)
            Criteria.PageSize = info.PageSize.Value;

        if (info.ToolSize != null)
            Toolbar.ShowCount = info.ToolSize.Value;
        Toolbar.Items = info.Tools?.Select(t => new ActionInfo(t)).ToList() ?? [];

        if (info.ActionSize != null)
            ActionCount = info.ActionSize.Value;
        Actions = info.Actions?.Select(a => new ActionInfo(a)).ToList() ?? [];

        var properties = TypeHelper.Properties(typeof(TItem));
        AllColumns = info.Columns?.OrderBy(t => t.Position).Select(c =>
        {
            var column = new ColumnInfo(c);
            SetColumn(column, model, form);
            var info = properties.FirstOrDefault(p => p.Name == column.Id);
            if (info != null)
                column.SetColumnInfo(info);
            return column;
        }).ToList();
        Columns.Clear();
        if (AllColumns != null && AllColumns.Count > 0)
            Columns.AddRange(AllColumns);

        SelectType = Toolbar.HasItem ? TableSelectType.Checkbox : TableSelectType.None;
    }

    private static void SetColumn(ColumnInfo column, EntityInfo model, FormInfo form)
    {
        var item = form?.Fields?.FirstOrDefault(f => f.Id == column.Id);
        if (item != null)
        {
            column.Type = item.Type;
            column.Category = item.Category;
        }
        if (column.Type == FieldType.Text)
        {
            var field = model?.Fields?.FirstOrDefault(f => f.Id == column.Id);
            if (field != null)
                column.Type = field.Type;
        }
    }

    private static List<ColumnInfo> GetAttributeColumns(Type type)
    {
        var columns = new List<ColumnInfo>();
        var properties = TypeHelper.Properties(typeof(TItem));
        foreach (var item in properties)
        {
            var attr = item.GetCustomAttribute<ColumnAttribute>();
            if (attr == null)
                continue;

            attr.Property = item;
            var column = new ColumnInfo(attr);
            columns.Add(column);
        }
        return columns;
    }
}