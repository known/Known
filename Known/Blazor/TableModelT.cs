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
        if (mode == TableColumnMode.Property)
            AllColumns = TypeHelper.Properties(typeof(TItem)).Select(p => new ColumnInfo(p)).ToList();
        else if (mode == TableColumnMode.Attribute)
            AllColumns = GetAttributeColumns(typeof(TItem));
        if (AllColumns != null && AllColumns.Count > 0)
            Columns = AllColumns;

        IsDictionary = typeof(TItem) == typeof(Dictionary<string, object>);
        OnAction = page.OnActionClick;
        Toolbar.OnItemClick = page.OnToolClick;
        Initialize(page is BasePage);
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
    /// 取得表格操作列是否有操作按钮。
    /// </summary>
    public bool HasAction => Actions != null && Actions.Count > 0;

    /// <summary>
    /// 取得表格是否有汇总字段列。
    /// </summary>
    public bool HasSum => Columns != null && Columns.Any(c => c.IsSum);

    /// <summary>
    /// 取得或设置表格是否显示工具条，默认显示。
    /// </summary>
    public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// 取得或设置表格是否显示列设置，默认显示。
    /// </summary>
    public bool ShowSetting { get; set; } = true;

    /// <summary>
    /// 取得或设置表格是否显示自动序号。
    /// </summary>
    public bool ShowIndex { get; set; }

    /// <summary>
    /// 取得或设置表格是否显示分页。
    /// </summary>
    public bool ShowPager { get; set; }

    /// <summary>
    /// 取得或设置表格是否可调整大小。
    /// </summary>
    public bool Resizable { get; set; }

    /// <summary>
    /// 取得或设置表格行是否显示斑马纹，默认显示。
    /// </summary>
    public bool IsStriped { get; set; } = true;

    /// <summary>
    /// 取得或设置表格是否是表单对话框的子表格。
    /// </summary>
    public bool IsForm { get; set; }

    /// <summary>
    /// 取得或设置表格选择列选择框类型。
    /// </summary>
    public TableSelectType SelectType { get; set; }

    /// <summary>
    /// 取得或设置表格名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置表格固定高度。
    /// </summary>
    public string FixedHeight { get; set; }

    /// <summary>
    /// 取得或设置表格操作列宽度，默认140。
    /// </summary>
    public string ActionWidth { get; set; } = "140";

    /// <summary>
    /// 取得或设置表格分页每页大小。
    /// </summary>
    public int? PageSize { get; set; }

    /// <summary>
    /// 取得或设置表格操作列显示按钮最大数量，默认2。
    /// </summary>
    public int ActionCount { get; set; } = 2;

    /// <summary>
    /// 取得或设置表格关联的表单配置信息。
    /// </summary>
    public FormInfo Form { get; set; }

    /// <summary>
    /// 取得或设置表格关联的自定义表单组件类型。
    /// </summary>
    public Type FormType { get; set; }

    /// <summary>
    /// 取得或设置表格关联的表单标题委托。
    /// </summary>
    public Func<TItem, string> FormTitle { get; set; }

    /// <summary>
    /// 取得表格标签配置对象。
    /// </summary>
    public TabModel Tab { get; } = new();

    /// <summary>
    /// 取得表格操作列信息列表。
    /// </summary>
    public List<ActionInfo> Actions { get; private set; } = [];

    /// <summary>
    /// 取得表格栏位呈现模板字典。
    /// </summary>
    public Dictionary<string, RenderFragment<TItem>> Templates { get; } = [];

    /// <summary>
    /// 取得或设置表格行数据主键委托。
    /// </summary>
    public Func<TItem, object> RowKey { get; set; }

    /// <summary>
    /// 取得或设置表格操作列事件委托。
    /// </summary>
    public Func<TItem, List<ActionInfo>> RowActions { get; set; }

    /// <summary>
    /// 取得或设置表格操作列根据数据更新按钮是否显示的委托。
    /// </summary>
    public Action<TItem, List<ActionInfo>> UpdateRowActions { get; set; }

    /// <summary>
    /// 取得或设置表格查询数据委托。
    /// </summary>
    public Func<PagingCriteria, Task<PagingResult<TItem>>> OnQuery { get; set; }

    /// <summary>
    /// 取得或设置表格行单击事件委托。
    /// </summary>
    public Func<TItem, Task> OnRowClick { get; set; }

    /// <summary>
    /// 取得或设置表格行双击事件委托。
    /// </summary>
    public Func<TItem, Task> OnRowDoubleClick { get; set; }

    /// <summary>
    /// 取得或设置表格操作列按钮单击事件委托。
    /// </summary>
    public Action<ActionInfo, TItem> OnAction { get; set; }

    /// <summary>
    /// 取得或设置表格刷新后调用的委托。
    /// </summary>
    public Action OnRefreshed { get; set; }

    /// <summary>
    /// 取得或设置树形表格子节点表达式。
    /// </summary>
    public Func<TItem, List<TItem>> TreeChildren { get; set; }

    /// <summary>
    /// 取得或设置表格顶部统计信息模板。
    /// </summary>
    public RenderFragment<PagingResult<TItem>> TopStatis { get; set; }

    /// <summary>
    /// 取得或设置表格行CSS类名委托。
    /// </summary>
    public Func<TItem, string> RowClass { get; set; }

    internal Func<PagingResult<TItem>, Task> OnRefreshStatis { get; set; }

    private List<TItem> dataSource = [];
    /// <summary>
    /// 取得或设置表格数据源。
    /// </summary>
    public List<TItem> DataSource
    {
        get { return dataSource; }
        set
        {
            dataSource = value ?? [];
            result = new PagingResult<TItem>(dataSource);
        }
    }

    private PagingResult<TItem> result = new();
    /// <summary>
    /// 取得或设置表格分页查询结果。
    /// </summary>
    public PagingResult<TItem> Result
    {
        get { return result; }
        set
        {
            result = value ?? new();
            dataSource = value?.PageData;
        }
    }

    internal void Initialize() => Initialize(true);

    /// <summary>
    /// 初始化表格栏位、权限、查询条件。
    /// </summary>
    /// <param name="isPage">是否是表格页面。</param>
    protected virtual void Initialize(bool isPage)
    {
        if (isPage)
        {
            var menu = Context?.Current;
            Name = Language.GetString(menu);
            Clear();
            SetPage(menu?.Model, menu?.Page);
            SetPermission();
        }

        SetQueryColumns();

        if (PageSize != null)
            Criteria.PageSize = PageSize.Value;
    }

    /// <summary>
    /// 清理表格，恢复默认数据。
    /// </summary>
    public void Clear()
    {
        Columns.Clear();
        QueryColumns.Clear();
        QueryData.Clear();
        Toolbar.Items?.Clear();
        Actions?.Clear();
        Criteria.Clear();
    }

    /// <summary>
    /// 添加操作列按钮。
    /// </summary>
    /// <param name="idOrName">按钮ID或名称。</param>
    public void AddAction(string idOrName) => Actions.Add(new ActionInfo(idOrName));

    /// <summary>
    /// 异步刷新表格数据统计。
    /// </summary>
    /// <returns></returns>
    public Task RefreshStatisAsync()
    {
        if (OnRefreshStatis == null)
            return Task.CompletedTask;

        return OnRefreshStatis.Invoke(Result);
    }

    /// <summary>
    /// 设置无代码页面信息。
    /// </summary>
    /// <param name="model">实体模型。</param>
    /// <param name="info">页面模型。</param>
    public void SetPage(EntityInfo model, PageInfo info)
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
        Toolbar.Items = info.Tools?.Select(t => new ActionInfo(t)).ToList();

        if (info.ActionSize != null)
            ActionCount = info.ActionSize.Value;
        Actions = info.Actions?.Select(a => new ActionInfo(a)).ToList();

        AllColumns = info.Columns?.Select(c =>
        {
            var column = new ColumnInfo(c);
            if (column.Type == FieldType.Text)
            {
                var field = model.Fields.FirstOrDefault(f => f.Id == c.Id);
                if (field != null)
                    column.Type = field.Type;
            }
            return column;
        }).ToList();
        Columns.Clear();
        Columns.AddRange(AllColumns);

        SelectType = Toolbar.HasItem ? TableSelectType.Checkbox : TableSelectType.None;
    }

    internal async Task PageRefreshAsync()
    {
        if (Page != null)
            await Page.RefreshAsync();
        else
            await RefreshAsync();

        OnRefreshed?.Invoke();
    }

    private bool ShowForm(FormModel<TItem> model)
    {
        model.Info = Form;
        model.Info ??= Context.Current.Form;
        model.Info ??= new FormInfo();
        return UI.ShowForm(model);
    }

    private void SetPermission()
    {
        if (Context == null)
            return;

        var menu = Context.Current;
        if (menu == null)
            return;

        Toolbar.Items = Toolbar.Items?.Where(t => menu.HasTool(t.Id)).ToList();
        Actions = Actions?.Where(a => menu.HasAction(a.Id)).ToList();

        var columns = Columns?.Where(c => menu.HasColumn(c.Id)).ToList();
        Columns.Clear();
        Columns.AddRange(columns);
        if (Columns != null && Columns.Count > 0)
        {
            var properties = TypeHelper.Properties(typeof(TItem));
            foreach (var item in Columns)
            {
                var info = properties.FirstOrDefault(p => p.Name == item.Id);
                if (info != null)
                    item.SetPropertyInfo(info);
            }
        }
    }

    private List<ColumnInfo> GetAttributeColumns(Type type)
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