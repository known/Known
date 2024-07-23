namespace Known.Blazor;

public class TableModel(UIContext context) : BaseModel(context)
{
    public bool AdvSearch { get; set; }
    public List<ColumnInfo> QueryColumns { get; } = [];
    public Dictionary<string, QueryInfo> QueryData { get; } = [];
    public PagingCriteria Criteria { get; } = new();
    public Func<Task> OnRefresh { get; set; }
    public ToolbarModel Toolbar { get; } = new();

    public bool HasToolbar => Toolbar != null && Toolbar.HasItem;
    internal virtual Type ItemType { get; }

    public Task RefreshAsync()
    {
        if (OnRefresh == null)
            return Task.CompletedTask;

        return OnRefresh.Invoke();
    }

    public void ShowAdvancedSearch(BaseLayout app)
    {
        AdvancedSearch search = null;
        var model = new DialogModel
        {
            Title = Language.AdvSearch,
            Width = 700,
            Content = b => b.Component<AdvancedSearch>()
                            .Set(c => c.ItemType, ItemType)
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
}

public class TableModel<TItem> : TableModel where TItem : class, new()
{
    public TableModel(BaseComponent page, bool isAuto = false) : base(page.Context)
    {
        AdvSearch = true;
        Page = page;
        if (isAuto)
        {
            AllColumns = TypeHelper.Properties(typeof(TItem)).Select(p => new ColumnInfo(p)).ToList();
            Columns = AllColumns;
            InitQueryColumns();
        }

        //OnAction = async (info, item) => await TypeHelper.ActionAsync(this, Context, page.App, info, [item]);
        //Toolbar.OnItemClick = async info => await TypeHelper.ActionAsync(this, Context, page.App, info, null);
        OnAction = page.OnActionClick;
        Toolbar.OnItemClick = page.OnToolClick;
    }

    internal List<ColumnInfo> AllColumns { get; private set; }
    internal SysModule Module { get; set; }
    internal string PageName => Language.GetString(Context.Current);
    internal override Type ItemType => typeof(TItem);

    public BaseComponent Page { get; }
    public bool IsDictionary => typeof(TItem) == typeof(Dictionary<string, object>);
    public bool HasAction => Actions != null && Actions.Count > 0;
    public bool HasSum => Columns != null && Columns.Any(c => c.IsSum);
    public bool ShowToolbar { get; set; } = true;
    public bool ShowPager { get; set; }
    public bool Resizable { get; set; }
    public bool IsForm { get; set; }
    public TableSelectType SelectType { get; set; }
    public string Name { get; set; }
    public string FixedWidth { get; set; }
    public string FixedHeight { get; set; }
    public string ActionWidth { get; set; } = "140";
    public int ActionCount { get; set; } = 2;
    public FormInfo Form { get; set; }
    public Type FormType { get; set; }
    public Func<TItem, string> FormTitle { get; set; }
    public TabModel Tab { get; } = new();
    public List<ColumnInfo> Columns { get; } = [];
    public List<ActionInfo> Actions { get; private set; } = [];
    public IEnumerable<TItem> SelectedRows { get; set; }
    public Dictionary<string, RenderFragment<TItem>> Templates { get; } = [];
    public Func<TItem, object> RowKey { get; set; }
    public Func<TItem, List<ActionInfo>> RowActions { get; set; }
    public Action<TItem, List<ActionInfo>> UpdateRowActions { get; set; }
    public Func<PagingCriteria, Task<PagingResult<TItem>>> OnQuery { get; set; }
    public Func<TItem, Task> OnRowClick { get; set; }
    public Action<ActionInfo, TItem> OnAction { get; set; }
    public Action OnRefreshed { get; set; }
    public Func<TItem, List<TItem>> TreeChildren { get; set; }
    public RenderFragment ToolbarSlot { get; set; }
    public Func<TItem, string> RowClass { get; set; }

    private List<TItem> dataSource = [];
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
    public PagingResult<TItem> Result
    {
        get { return result; }
        set
        {
            result = value ?? new();
            dataSource = value?.PageData;
        }
    }

    public void Clear()
    {
        Columns.Clear();
        QueryColumns.Clear();
        QueryData.Clear();
        Toolbar.Items?.Clear();
        Actions?.Clear();
        Criteria.Clear();
    }

    public ColumnBuilder<TItem> Column<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var property = TypeHelper.Property(selector);
        var column = Columns?.FirstOrDefault(c => c.Id == property.Name);
        return new ColumnBuilder<TItem>(column, this);
    }

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

    public void AddQueryColumn(Expression<Func<TItem, object>> selector)
    {
        var property = TypeHelper.Property(selector);
        if (QueryColumns.Exists(c => c.Id == property.Name))
            return;

        var column = new ColumnInfo(property);
        QueryColumns.Add(column);
        QueryData[property.Name] = new QueryInfo(column);
    }

    public void AddAction(string idOrName) => Actions.Add(new ActionInfo(idOrName));
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

    public void NewForm(Func<TItem, Task<Result>> onSave, TItem row) => ShowForm("New", onSave, row);
    public void NewForm(Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row) => ShowForm("New", onSave, row);
    public void EditForm(Func<TItem, Task<Result>> onSave, TItem row) => ShowForm("Edit", onSave, row);
    public void EditForm(Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row) => ShowForm("Edit", onSave, row);
    public void DeleteM(Func<List<TItem>, Task<Result>> action) => SelectRows(action, "Delete");

    public void Delete(Func<List<TItem>, Task<Result>> action, TItem row)
    {
        UI.Confirm(Language?["Tip.ConfirmDeleteRecord"], async () =>
        {
            var result = await action?.Invoke([row]);
            UI.Result(result, PageRefreshAsync);
        });
    }

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

    public virtual void Initialize(BasePage page)
    {
        Clear();
        SetPage(page.Context.Current.Page);
        SetPermission(page);
        InitQueryColumns();
    }

    internal void SetPage(PageInfo info)
    {
        if (info == null)
            return;

        FixedWidth = info.FixedWidth;
        FixedHeight = info.FixedHeight;
        ShowPager = info.ShowPager;

        Toolbar.Items = info.Tools?.Select(t => new ActionInfo(t)).ToList();
        Actions = info.Actions?.Select(a => new ActionInfo(a)).ToList();
        AllColumns = info.Columns?.Select(c => new ColumnInfo(c)).ToList();
        Columns.Clear();
        Columns.AddRange(AllColumns);

        SelectType = Toolbar.HasItem ? TableSelectType.Checkbox : TableSelectType.None;
        InitQueryColumns();
    }

    internal async Task PageRefreshAsync()
    {
        if (Page != null)
            await Page.RefreshAsync();
        else
            await RefreshAsync();

        OnRefreshed?.Invoke();
    }

    private void ShowForm(string actionName, Func<TItem, Task<Result>> onSave, TItem row)
    {
        ShowForm(new FormModel<TItem>(this) { Action = actionName, Data = row, OnSave = onSave });
    }

    private void ShowForm(string actionName, Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row)
    {
        ShowForm(new FormModel<TItem>(this) { Action = actionName, Data = row, OnSaveFile = onSave });
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

    private void InitQueryColumns()
    {
        QueryColumns.Clear();
        if (Columns != null && Columns.Count > 0)
            QueryColumns.AddRange(Columns.Where(c => c.IsQuery));

        if (QueryColumns != null && QueryColumns.Count > 0)
        {
            foreach (var item in QueryColumns)
            {
                QueryData[item.Id] = new QueryInfo(item);
            }
        }
    }

    private string GetConfirmText(string text)
    {
        text = Language.GetText("Button", text);
        return Language?["Tip.ConfirmRecordName"]?.Replace("{text}", text);
    }
}