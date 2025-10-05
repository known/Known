using AntDesign;

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
    public TableModel(IBaseComponent page, TableColumnMode mode = TableColumnMode.None) : this(page, null, mode) { }

    /// <summary>
    /// 构造函数，创建一个泛型表格组件模型信息类的实例。
    /// </summary>
    /// <param name="page">表格关联的页面组件。</param>
    /// <param name="id">表格ID。</param>
    /// <param name="mode">根据数据类型自动生成表格列。</param>
    public TableModel(IBaseComponent page, string id, TableColumnMode mode = TableColumnMode.None) : base(page, id)
    {
        InitializeTab();
        IsAuto = mode != TableColumnMode.None;
        AdvSearch = true;
        IsDictionary = typeof(TItem).IsDictionary();
        if (mode == TableColumnMode.Property)
            AllColumns = TypeCache.Model(typeof(TItem)).GetColumns(false);
        else if (mode == TableColumnMode.Attribute)
            AllColumns = TypeCache.Model(typeof(TItem)).GetColumns(true);

        if (page != null)
        {
            OnAction = (info, item) => Context.OnAction(page, info, [item]);
            Toolbar.OnItemClick = info => Context.OnAction(page, info, null);
        }

        var isPage = !IsAuto && page is BasePage;
        Initialize(isPage);
    }

    /// <summary>
    /// 取得表格用户列设置ID。
    /// </summary>
    public string SettingId => $"UserTable_{Id}";

    /// <summary>
    /// 取得或设置表格关联的页面。
    /// </summary>
    public BaseTablePage TablePage { get; set; }

    /// <summary>
    /// 取得表格数据是否是字典类型。
    /// </summary>
    public bool IsDictionary { get; }

    /// <summary>
    /// 取得或设置表格是否显示工具条，默认显示。
    /// </summary>
    public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// 取得或设置树形表格子节点表达式。
    /// </summary>
    public Func<TItem, List<TItem>> TreeChildren { get; set; }

    /// <summary>
    /// 取得或设置初始化表格其他属性参数委托。
    /// </summary>
    public Action<Table<TItem>> OnInitial { get; set; }

    internal override string TableId => typeof(TItem).Name;
    internal override Type DataType => typeof(TItem);

    /// <summary>
    /// 初始化表格插件。
    /// </summary>
    public void Initialize()
    {
        var info = MenuHelper.CreateAutoPage(Page.GetType());
        Initialize(info);
    }

    /// <summary>
    /// 初始化页面表格模型配置。
    /// </summary>
    /// <param name="info">表格配置模型信息。</param>
    public void Initialize(AutoPageInfo info)
    {
        Info = info;
        Name = info?.Name ?? info?.Page?.Name;
        AdvSearch = info?.Page?.ShowAdvSearch == true;
        ShowPager = info?.Page?.ShowPager == true;
        ShowSetting = info?.Page?.ShowSetting == true;

        if (info?.Page?.PageSize != null)
            Criteria.PageSize = info.Page.PageSize.Value;
        if (info?.Page?.ToolSize != null)
            Toolbar.ShowCount = info.Page.ToolSize.Value;
        if (info?.Page?.ActionSize != null)
            ActionCount = info.Page.ActionSize.Value;

        Toolbar.Items = info?.Page?.Tools ?? [];
        Actions = info?.Page?.Actions ?? [];
        AllColumns = info?.Page?.GetColumns<TItem>();
        SelectType = Toolbar.HasItem ? TableSelectType.Checkbox : TableSelectType.None;

        Columns.Clear();
        if (AllColumns != null && AllColumns.Count > 0)
            Columns.AddRange(AllColumns);

        SetQueryColumns();
    }

    /// <summary>
    /// 初始化表格栏位、权限、查询条件。
    /// </summary>
    /// <param name="isPage">是否是表格页面。</param>
    public virtual void Initialize(bool isPage)
    {
        if (isPage)
        {
            Clear();
            var menu = Context?.Current;
            if (menu != null)
            {
                var info = menu.GetAutoPageParameter();
                Initialize(info);
                if (string.IsNullOrWhiteSpace(Name))
                    Name = Language.GetString(menu);
            }
        }

        Columns = GetUserColumns();
        SetQueryColumns();
        if (PageSize != null)
            Criteria.PageSize = PageSize.Value;
    }

    /// <summary>
    /// 清理表格，恢复默认数据。
    /// </summary>
    public void Clear()
    {
        AllColumns?.Clear();
        Columns?.Clear();
        QueryColumns?.Clear();
        QueryData?.Clear();
        Toolbar?.Items?.Clear();
        Actions?.Clear();
        Criteria?.Clear();
    }
}