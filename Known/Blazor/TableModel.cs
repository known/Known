namespace Known.Blazor;

/// <summary>
/// 表格组件模型信息类。
/// </summary>
/// <param name="context">UI上下文对象。</param>
public class TableModel(UIContext context) : BaseModel(context)
{
    /// <summary>
    /// 取得或设置表格是否显示高级搜索。
    /// </summary>
    public bool AdvSearch { get; set; }

    /// <summary>
    /// 取得或设置表格默认查询条件匿名对象，对象属性名应与查询实体对应。
    /// </summary>
    public object DefaultQuery { get; set; }

    /// <summary>
    /// 取得表格查询栏位信息列表。
    /// </summary>
    public List<ColumnInfo> QueryColumns { get; } = [];

    /// <summary>
    /// 取得表格查询数据信息字典。
    /// </summary>
    public Dictionary<string, QueryInfo> QueryData { get; } = [];

    /// <summary>
    /// 取得表的查询条件对象。
    /// </summary>
    public PagingCriteria Criteria { get; } = new();

    /// <summary>
    /// 取得或设置表格刷新委托，创建抽象表格时赋值。
    /// </summary>
    public Func<Task> OnRefresh { get; set; }

    /// <summary>
    /// 取得表格工具条配置模型对象。
    /// </summary>
    public ToolbarModel Toolbar { get; } = new();

    /// <summary>
    /// 取得表格是否有工具条按钮。
    /// </summary>
    public bool HasToolbar => Toolbar != null && Toolbar.HasItem;

    internal List<ColumnInfo> AllColumns { get; set; }

    /// <summary>
    /// 刷新表格数据。
    /// </summary>
    /// <returns></returns>
    public Task RefreshAsync()
    {
        if (OnRefresh == null)
            return Task.CompletedTask;

        return OnRefresh.Invoke();
    }

    /// <summary>
    /// 显示高级搜索对话框。
    /// </summary>
    /// <param name="app">系统模板对象。</param>
    public void ShowAdvancedSearch(BaseLayout app)
    {
        AdvancedSearch search = null;
        var model = new DialogModel
        {
            Title = Language.AdvSearch,
            Width = 700,
            Content = b => b.Component<AdvancedSearch>()
                            .Set(c => c.Columns, AllColumns)
                            .Build(value => search = value)
        };
        model.OnOk = async () =>
        {
            await app.QueryDataAsync(async () =>
            {
                Criteria.Query = await search?.SaveQueryAsync();
                await RefreshAsync();
            });
            await model.CloseAsync();
        };
        UI.ShowDialog(model);
    }

    /// <summary>
    /// 设置默认查询条件数据。
    /// </summary>
    public void SetDefaultQuery()
    {
        QueryData.Clear();
        if (QueryColumns == null || QueryColumns.Count == 0)
            return;

        foreach (var item in QueryColumns)
        {
            var info = new QueryInfo(item);
            info.Value = TypeHelper.GetPropertyValue<string>(DefaultQuery, item.Id);
            QueryData[item.Id] = info;
        }

        Criteria.Query = QueryData.Select(d => d.Value).ToList();
    }
}

/// <summary>
/// 泛型表格组件模型信息类。
/// </summary>
/// <typeparam name="TItem">表格行数据类型。</typeparam>
public class TableModel<TItem> : TableModel where TItem : class, new()
{
    /// <summary>
    /// 构造函数，创建一个泛型表格组件模型信息类的实例。
    /// </summary>
    /// <param name="page">页面组件对象。</param>
    /// <param name="isAuto">是否根据数据类型自动生成表格列。</param>
    public TableModel(BaseComponent page, bool isAuto = false) : base(page.Context)
    {
        AdvSearch = true;
        Page = page;
        if (isAuto)
        {
            AllColumns = TypeHelper.Properties(typeof(TItem)).Select(p => new ColumnInfo(p)).ToList();
            Columns = AllColumns;
        }

        IsDictionary = typeof(TItem) == typeof(Dictionary<string, object>);
        OnAction = page.OnActionClick;
        Toolbar.OnItemClick = page.OnToolClick;
    }

    internal SysModule Module { get; set; }

    /// <summary>
    /// 获取页面名称。
    /// </summary>
    public string PageName => Language.GetString(Context.Current);

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
    /// 取得或设置表格是否显示分页。
    /// </summary>
    public bool ShowPager { get; set; }

    /// <summary>
    /// 取得或设置表格是否可调整大小。
    /// </summary>
    public bool Resizable { get; set; }

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
    /// 取得表格栏位信息列表。
    /// </summary>
    public List<ColumnInfo> Columns { get; } = [];

    /// <summary>
    /// 取得表格操作列信息列表。
    /// </summary>
    public List<ActionInfo> Actions { get; private set; } = [];

    /// <summary>
    /// 取得或设置表格选中行绑定的数据列表。
    /// </summary>
    public IEnumerable<TItem> SelectedRows { get; set; }

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

    /// <summary>
    /// 初始化表格栏位、权限、查询条件。
    /// </summary>
    /// <param name="page">页面组件对象。</param>
    public virtual void Initialize(BasePage page)
    {
        var menu = page?.Context?.Current;
        Clear();
        SetPage(menu?.Model, menu?.Page);
        SetPermission(page);
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
    /// 获取表格栏位建造者对象。
    /// </summary>
    /// <typeparam name="TValue">栏位属性类型。</typeparam>
    /// <param name="selector">栏位属性选择表达式。</param>
    /// <returns>栏位建造者对象。</returns>
    public ColumnBuilder<TItem> Column<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var property = TypeHelper.Property(selector);
        var column = Columns?.FirstOrDefault(c => c.Id == property.Name);
        return new ColumnBuilder<TItem>(column, this);
    }

    /// <summary>
    /// 添加一个表格栏位。
    /// </summary>
    /// <typeparam name="TValue">栏位属性类型。</typeparam>
    /// <param name="selector">栏位属性选择表达式。</param>
    /// <param name="isQuery">是否是查询字段。</param>
    /// <returns>栏位建造者对象。</returns>
    public ColumnBuilder<TItem> AddColumn<TValue>(Expression<Func<TItem, TValue>> selector, bool isQuery = false)
    {
        var property = TypeHelper.Property(selector);
        var column = new ColumnInfo(property);
        Columns.Add(column);
        if (isQuery)
        {
            QueryColumns.Add(column);
            QueryData[property.Name] = new QueryInfo(column);
        }
        return new ColumnBuilder<TItem>(column, this);
    }

    /// <summary>
    /// 添加额外查询条件字段。
    /// </summary>
    /// <param name="selector">栏位属性选择表达式。</param>
    public void AddQueryColumn(Expression<Func<TItem, object>> selector)
    {
        var property = TypeHelper.Property(selector);
        if (QueryColumns.Exists(c => c.Id == property.Name))
            return;

        var column = new ColumnInfo(property);
        QueryColumns.Add(column);
        QueryData[property.Name] = new QueryInfo(column);
    }

    /// <summary>
    /// 添加操作列按钮。
    /// </summary>
    /// <param name="idOrName">按钮ID或名称。</param>
    public void AddAction(string idOrName) => Actions.Add(new ActionInfo(idOrName));
    
    /// <summary>
    /// 显示查看表单对话框。
    /// </summary>
    /// <param name="row">查看行绑定的对象。</param>
    public void ViewForm(TItem row) => ViewForm(FormViewType.View, row);

    internal void ViewForm(FormViewType type, TItem row)
    {
        ShowForm(new FormModel<TItem>(this)
        {
            FormType = type,
            IsView = true,
            Action = $"{type}",
            Data = row
        });
    }

    /// <summary>
    /// 显示新增表单对话框。
    /// </summary>
    /// <param name="onSave">新增保存方法委托。</param>
    /// <param name="row">新增默认对象。</param>
    public void NewForm(Func<TItem, Task<Result>> onSave, TItem row)
    {
        var model = new FormModel<TItem>(this) { Action = "New", DefaultData = row, OnSave = onSave };
        model.LoadDefaultData();
        ShowForm(model);
    }

    /// <summary>
    /// 显示带有附件的新增表单对话框。
    /// </summary>
    /// <param name="onSave">新增保存方法委托。</param>
    /// <param name="row">新增默认对象。</param>
    public void NewForm(Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row)
    {
        var model = new FormModel<TItem>(this) { Action = "New", DefaultData = row, OnSaveFile = onSave };
        model.LoadDefaultData();
        ShowForm(model);
    }

    /// <summary>
    /// 显示新增表单对话框。
    /// </summary>
    /// <param name="onSave">新增保存方法委托。</param>
    /// <param name="row">异步请求默认对象委托。</param>
    public async void NewForm(Func<TItem, Task<Result>> onSave, Func<Task<TItem>> row)
    {
        var model = new FormModel<TItem>(this) { Action = "New", DefaultDataAction = row, OnSave = onSave };
        await model.LoadDefaultDataAsync();
        ShowForm(model);
    }

    /// <summary>
    /// 显示带有附件的新增表单对话框。
    /// </summary>
    /// <param name="onSave">新增保存方法委托。</param>
    /// <param name="row">异步请求默认对象委托。</param>
    public async void NewForm(Func<UploadInfo<TItem>, Task<Result>> onSave, Func<Task<TItem>> row)
    {
        var model = new FormModel<TItem>(this) { Action = "New", DefaultDataAction = row, OnSaveFile = onSave };
        await model.LoadDefaultDataAsync();
        ShowForm(model);
    }

    /// <summary>
    /// 显示编辑表单对话框。
    /// </summary>
    /// <param name="onSave">编辑保存方法委托。</param>
    /// <param name="row">编辑行绑定的对象。</param>
    public void EditForm(Func<TItem, Task<Result>> onSave, TItem row)
    {
        ShowForm(new FormModel<TItem>(this) { Action = "Edit", Data = row, OnSave = onSave });
    }

    /// <summary>
    /// 显示带有附件的编辑表单对话框。
    /// </summary>
    /// <param name="onSave">编辑保存方法委托。</param>
    /// <param name="row">编辑行绑定的对象。</param>
    public void EditForm(Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row)
    {
        ShowForm(new FormModel<TItem>(this) { Action = "Edit", Data = row, OnSaveFile = onSave });
    }

    /// <summary>
    /// 批量删除表格数据。
    /// </summary>
    /// <param name="action">删除方法委托。</param>
    public void DeleteM(Func<List<TItem>, Task<Result>> action) => SelectRows(action, "Delete");

    /// <summary>
    /// 删除表格一行数据。
    /// </summary>
    /// <param name="action">删除一条数据方法委托。</param>
    /// <param name="row">删除行绑定的对象。</param>
    public void Delete(Func<TItem, Task<Result>> action, TItem row)
    {
        UI.Confirm(Language?["Tip.ConfirmDeleteRecord"], async () =>
        {
            var result = await action?.Invoke(row);
            UI.Result(result, PageRefreshAsync);
        });
    }

    /// <summary>
    /// 删除表格一行数据。
    /// </summary>
    /// <param name="action">删除多条数据方法委托。</param>
    /// <param name="row">删除行绑定的对象。</param>
    public void Delete(Func<List<TItem>, Task<Result>> action, TItem row)
    {
        UI.Confirm(Language?["Tip.ConfirmDeleteRecord"], async () =>
        {
            var result = await action?.Invoke([row]);
            UI.Result(result, PageRefreshAsync);
        });
    }

    /// <summary>
    /// 异步弹窗显示导入表单。
    /// </summary>
    /// <param name="param">与后端对应的导入参数。</param>
    /// <returns></returns>
    public async Task ShowImportsync(string param = null)
    {
        var type = typeof(TItem);
        var id = $"{type.Name}Import";
        if (!string.IsNullOrWhiteSpace(param))
            id += $"_{param}";
        if (IsDictionary)
            id += $"_{Context.Current.Id}";
        var importTitle = Language.GetImportTitle(PageName);
        var fileService = await Page.CreateServiceAsync<IFileService>();
        var info = await fileService.GetImportAsync(id);
        info.Name = PageName;
        info.BizName = importTitle;
        var model = new DialogModel { Title = importTitle };
        model.Content = builder =>
        {
            builder.Component<Importer>()
                   .Set(c => c.Model, info)
                   .Set(c => c.OnSuccess, async () =>
                   {
                       await model.CloseAsync();
                       await RefreshAsync();
                   })
                   .Build();
        };
        UI.ShowDialog(model);
    }

    /// <summary>
    /// 根据导出模式异步导出表格数据，默认按查询结果导出。
    /// </summary>
    /// <param name="mode">导出模式（单页，查询结果，全部）。</param>
    /// <returns></returns>
    public Task ExportDataAsync(ExportMode mode = ExportMode.Query) => ExportDataAsync(PageName, mode);

    /// <summary>
    /// 异步导出表格数据，默认按查询结果导出。
    /// </summary>
    /// <param name="name">导出文件名。</param>
    /// <param name="mode">导出模式（单页，查询结果，全部）。</param>
    /// <returns></returns>
    public Task ExportDataAsync(string name, ExportMode mode = ExportMode.Query) => Page.App?.ExportDataAsync(this, name, mode);

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
    /// 选择表格一行数据操作。
    /// </summary>
    /// <param name="action">操作方法委托。</param>
    public void SelectRow(Action<TItem> action)
    {
        if (SelectedRows == null)
        {
            UI.Warning(Language?.SelectOne);
            return;
        }

        var rows = SelectedRows.ToList();
        if (rows.Count == 0 || rows.Count > 1)
        {
            UI.Warning(Language?.SelectOne);
            return;
        }

        action?.Invoke(rows[0]);
    }

    /// <summary>
    /// 选择表格一行数据，带确认对话框的操作。
    /// </summary>
    /// <param name="action">操作方法委托。</param>
    /// <param name="confirmText">确认对话框文本。</param>
    public void SelectRow(Func<TItem, Task<Result>> action, string confirmText = null)
    {
        SelectRow(async row =>
        {
            if (!string.IsNullOrWhiteSpace(confirmText))
            {
                UI.Confirm(GetConfirmText(confirmText), async () =>
                {
                    var result = await action?.Invoke(row);
                    UI.Result(result, PageRefreshAsync);
                });
            }
            else
            {
                var result = await action?.Invoke(row);
                UI.Result(result, PageRefreshAsync);
            }
        });
    }

    /// <summary>
    /// 选择表格多行数据操作。
    /// </summary>
    /// <param name="action">操作方法委托。</param>
    public void SelectRows(Action<List<TItem>> action)
    {
        if (SelectedRows == null)
        {
            UI.Warning(Language?.SelectOneAtLeast);
            return;
        }

        var rows = SelectedRows.ToList();
        if (rows.Count == 0)
        {
            UI.Warning(Language?.SelectOneAtLeast);
            return;
        }

        action?.Invoke(rows);
    }

    /// <summary>
    /// 选择表格多行数据，带确认对话框的操作。
    /// </summary>
    /// <param name="action">操作方法委托。</param>
    /// <param name="confirmText">确认对话框文本。</param>
    public void SelectRows(Func<List<TItem>, Task<Result>> action, string confirmText = null)
    {
        SelectRows(async rows =>
        {
            if (!string.IsNullOrWhiteSpace(confirmText))
            {
                UI.Confirm(GetConfirmText(confirmText), async () =>
                {
                    var result = await action?.Invoke(rows);
                    UI.Result(result, PageRefreshAsync);
                });
            }
            else
            {
                var result = await action?.Invoke(rows);
                UI.Result(result, PageRefreshAsync);
            }
        });
    }

    internal void SetPage(EntityInfo model, PageInfo info)
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
            var field = model.Fields.FirstOrDefault(f => f.Id == c.Id);
            if (field != null)
                column.Type = field.Type;
            return column;
        }).ToList();
        Columns.Clear();
        Columns.AddRange(AllColumns);

        SelectType = Toolbar.HasItem ? TableSelectType.Checkbox : TableSelectType.None;
    }

    internal void SetQueryColumns()
    {
        QueryColumns.Clear();
        if (Columns != null && Columns.Count > 0)
            QueryColumns.AddRange(Columns.Where(c => c.IsQuery));

        SetDefaultQuery();
    }

    internal async Task PageRefreshAsync()
    {
        if (Page != null)
            await Page.RefreshAsync();
        else
            await RefreshAsync();

        OnRefreshed?.Invoke();
    }

    private void ShowForm(FormModel<TItem> model)
    {
        model.Info = Form;
        model.Info ??= Context.Current.Form;
        model.Info ??= new FormInfo();
        UI.ShowForm(model);
    }

    private void SetPermission(BasePage page)
    {
        if (page == null || page.Context == null)
            return;

        var menu = page.Context.Current;
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

    private string GetConfirmText(string text)
    {
        text = Language.GetText("Button", text);
        return Language?["Tip.ConfirmRecordName"]?.Replace("{text}", text);
    }
}