using Known.Razor.Pages.Forms;

namespace Known.Razor.Components;

public class DataGrid<TItem> : DataComponent<TItem>
{
    private readonly string qvAdvQueryId;
    internal List<Column<TItem>> GridColumns;
    internal string GridId;
    internal int CurRow = -1;
    internal bool CheckAll = false;

    public DataGrid()
    {
        var id = Utils.GetGuid();
        qvAdvQueryId = $"qv-{id}";
        GridId = $"dg-{id}";
        ContainerStyle = "grid-view";
        ContentStyle = "grid";
        InitMenu();
    }

    [Parameter] public Action<object> OnPicked { get; set; }

    public List<Column<TItem>> Columns { get; set; }
    public List<ButtonInfo> Actions { get; set; }
    public Action<RenderTreeBuilder> ActionHead { get; set; }
    public Action<RenderTreeBuilder> CustHead { get; set; }
    public bool IsEdit { get; set; }
    protected bool IsFixed { get; set; } = true;
    public bool IsSort { get; set; } = true;
    public bool ShowEmpty { get; set; } = true;
    public bool ShowSetting { get; set; }
    public bool ShowCheckBox { get; set; }
    public string OrderBy { get; set; }
    internal string RowTitle { get; set; }

    public virtual void View(TItem item) { }
    public virtual void Import() => ShowImport(Name, typeof(TItem));
    public virtual void Export(ExportMode mode = ExportMode.Query, string extension = null) => ExportData(Name, mode, extension);

    public override void Refresh()
    {
        query = null;
        QueryData();
    }

    protected void SetGridPicker()
    {
        ShowSetting = false;
        ShowCheckBox = false;
        RowTitle = "双击选择数据。";
        if (HasButton(ToolButton.New))
            Tools = new List<ButtonInfo> { ToolButton.New };
        Actions = null;
        Columns.ForEach(c => c.IsAdvQuery = false);
    }

    protected void SetEdit(bool isEdit)
    {
        IsEdit = isEdit;
        foreach (var item in GridColumns)
        {
            item.IsEdit = isEdit;
        }
        StateChanged();
    }

    public void SetColumn(string id, bool isVisible)
    {
        if (GridColumns == null || GridColumns.Count == 0)
            return;

        var column = GridColumns.FirstOrDefault(c => c.Id == id);
        if (column == null)
            return;

        column.IsVisible = isVisible;
        StateChanged();
    }

    protected void SetColumns(List<Column<TItem>> columns)
    {
        GridColumns = columns;
        ShowQuery = GridColumns != null && GridColumns.Any(c => c.IsQuery);
        StateChanged();
    }

    protected ColumnBuilder<TItem> Column<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var property = TypeHelper.Property(selector);
        var column = Columns?.FirstOrDefault(c => c.Id == property.Name);
        if (column == null)
            return new ColumnBuilder<TItem>();

        return new ColumnBuilder<TItem>(column);
    }

    protected override List<string> GetSumColumns()
    {
        if (GridColumns == null || GridColumns.Count == 0)
            return null;

        return GridColumns.Where(c => c.IsSum).Select(c => c.Id).ToList();
    }

    protected void ShowForm<T>(string title, object model, Size? size = null, Action<AttributeBuilder<T>> action = null) where T : Form
    {
        UI.ShowForm(title, model, CloseForm, size, action);
    }

    protected virtual void CloseForm(Result result)
    {
        if (result.IsClose)
            UI.CloseDialog();

        Refresh();
    }

    public virtual bool CheckAction(ButtonInfo action, TItem item) => true;
    public virtual void OnRowClick(int row, TItem item) { }
    public virtual void OnRowDoubleClick(int row, TItem item) { }

    protected void SelectRow(Action<TItem> action)
    {
        var selected = SelectedItems;
        if (!selected.Any() || selected.Count > 1)
        {
            UI.Toast(Language.SelectOne);
            return;
        }

        action.Invoke(selected[0]);
    }

    protected void SelectRows(Action<List<TItem>> action)
    {
        var selected = SelectedItems;
        if (!selected.Any())
        {
            UI.Toast(Language.SelectOneAtLeast);
            return;
        }

        action.Invoke(selected);
    }

    protected void DeleteRow(TItem item, Func<List<TItem>, Task<Result>> action)
    {
        UI.Confirm("确定要删除？", async () =>
        {
            var result = await action.Invoke(new List<TItem> { item });
            UI.Result(result, Refresh);
        });
    }

    protected void DeleteRows(Func<List<TItem>, Task<Result>> action)
    {
        SelectRows(items =>
        {
            UI.Confirm($"确定要删除选中的{items.Count}条记录？", async () =>
            {
                var result = await action?.Invoke(items);
                UI.Result(result, Refresh);
            });
        });
    }

    protected async void MoveRow(TItem item, bool isMoveUp, Func<TItem, Task<Result>> action = null, Action<TItem, TItem> success = null)
    {
        var index = Data.IndexOf(item);
        var index1 = isMoveUp ? index - 1 : index + 1;
        if (index1 < 0 || index1 > Data.Count - 1)
            return;

        if (action != null)
        {
            var result = await action(item);
            if (result.IsValid)
                OnMoveRow(item, success, index, index1);
        }
        else
        {
            OnMoveRow(item, success, index, index1);
        }
    }

    protected void ImportList(string headId) => ShowImport(Name, typeof(TItem), headId);

    protected async void ShowImport(string name, Type type, string param = null)
    {
        var id = $"{type.Name}Import";
        if (!string.IsNullOrWhiteSpace(param))
            id += $"_{param}";
        var model = await Platform.File.GetImportAsync(id);
        model.BizName = $"导入{name}";
        model.Type = type?.AssemblyQualifiedName;
        UI.ShowImport(new ImportOption { Id = id, Name = name, Model = model });
    }

    protected override Task InitPageAsync()
    {
        if (OnPicked != null)
            SetGridPicker();

        GridColumns = Setting.GetUserColumns(Id, Columns);
        ShowQuery = GridColumns != null && GridColumns.Any(c => c.IsQuery);

        if (!string.IsNullOrWhiteSpace(OrderBy))
            OrderBys = new string[] { OrderBy };

        return base.InitPageAsync();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (IsFixed)
            UI.FixedTable(GridId);

        return base.OnAfterRenderAsync(firstRender);
    }

    internal override void BuildContent(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("table").AddClass("fixed", IsFixed).Build();
        builder.Div(css, attr =>
        {
            builder.Component<CascadingValue<DataGrid<TItem>>>(attr =>
            {
                attr.Set(c => c.IsFixed, false)
                    .Set(c => c.Value, this)
                    .Set(c => c.ChildContent, b => b.Component<Table<TItem>>().Build());
            });
        });
    }

    internal override void BuildPager(RenderTreeBuilder builder)
    {
        base.BuildPager(builder);
        BuildAdvQuery(builder);
    }

    protected override void BuildQuerys(RenderTreeBuilder builder)
    {
        var columns = GridColumns?.Where(c => c.IsQuery).ToList();
        if (columns == null || columns.Count == 0)
            return;

        foreach (var item in columns)
        {
            item.BuildQuery(builder, this);
        }
        builder.Button(FormButton.Query, Callback(() =>
        {
            query = null;
            QueryData(true);
        }));

        if (GridColumns != null && GridColumns.Any(c => c.IsAdvQuery))
        {
            builder.Button(FormButton.AdvQuery, Callback(ShowAdvQuery), style: "qvtrigger");
        }
    }

    private void InitMenu()
    {
        var menu = KRConfig.UserMenus.FirstOrDefault(m => m.Code == Id);
        if (menu != null)
        {
            Id = menu.Id;
            Name = menu.Name;
            if (menu.Buttons != null && menu.Buttons.Count > 0)
                Tools = menu.Buttons.Select(n => ToolButton.Buttons.FirstOrDefault(b => b.Name == n)).ToList();
            if (menu.Actions != null && menu.Actions.Count > 0)
                Actions = menu.Actions.Select(n => GridAction.Actions.FirstOrDefault(b => b.Name == n)).ToList();
            Columns = menu.Columns?.Select(c => new Column<TItem>(c)).ToList();
        }
    }

    internal bool HasFoot()
    {
        if (GridColumns == null || GridColumns.Count == 0)
            return false;

        var columns = GridColumns.Where(c => c.IsSum).ToList();
        if (columns != null && columns.Count > 0)
            return true;

        return false;
    }

    internal bool HasAction()
    {
        if (IsEdit && ReadOnly)
            return false;

        if (Actions == null || Actions.Count == 0)
            return false;

        return true;
    }

    private void OnMoveRow(TItem item, Action<TItem, TItem> success, int index, int index1)
    {
        CurRow = index1;
        var temp = Data[index1];
        Data[index1] = item;
        Data[index] = temp;
        success?.Invoke(item, temp);
        StateChanged();
    }

    internal void OnSort(ColumnInfo item, string curOrder)
    {
        OrderBys = new string[] { $"{item.Id} {curOrder}" };
        QueryData();
    }

    private void BuildAdvQuery(RenderTreeBuilder builder)
    {
        if (GridColumns == null || !GridColumns.Any(c => c.IsAdvQuery))
            return;

        builder.Component<QuickView>()
               .Set(c => c.Id, qvAdvQueryId)
               .Set(c => c.Style, "query-adv")
               .Set(c => c.ChildContent, b =>
               {
                   b.Component<AdvQuery<TItem>>()
                    .Set(c => c.PageId, Id)
                    .Set(c => c.Columns, Columns)
                    .Set(c => c.OnSetting, value =>
                    {
                        query = value;
                        QueryData(true);
                    })
                    .Build();
               })
               .Build();
    }

    private void ShowAdvQuery() => UI.ShowQuickView(qvAdvQueryId);

    internal void ShowColumnSetting()
    {
        var data = Columns.Select(c => c.ToColumn()).ToList();
        UI.Show<ColumnGrid>("表格设置", new(780, 500), action: attr =>
        {
            attr.Set(c => c.PageId, Id)
                .Set(c => c.Data, data)
                .Set(c => c.OnSetting, () =>
                {
                    var columns = Setting.GetUserColumns(Id, Columns);
                    SetColumns(columns);
                });
        });
    }

    private Dictionary<string, string> GetExportColumns()
    {
        var columns = new Dictionary<string, string>();
        if (GridColumns != null && GridColumns.Count > 0)
        {
            foreach (var item in GridColumns)
            {
                columns.Add(item.Id, item.Name);
            }
        }
        return columns;
    }

    private async void ExportData(string name, ExportMode mode, string extension = null)
    {
        criteria.ExportMode = mode;
        criteria.ExportExtension = extension;
        criteria.ExportColumns = GetExportColumns();
        var result = await OnQueryData(criteria);
        var bytes = result.ExportData;
        if (bytes == null || bytes.Length == 0)
        {
            UI.Alert("无数据可导出！");
            return;
        }

        var stream = new MemoryStream(bytes);
        UI.DownloadFile($"{name}.xlsx", stream);
    }
}

public class DataGrid<TItem, TForm> : DataGrid<TItem> where TItem : EntityBase, new() where TForm : Form
{
    public DataGrid()
    {
        ShowSetting = true;
        ShowCheckBox = true;
    }

    protected virtual Task<TItem> GetDefaultModelAsync() => Task.FromResult(new TItem());

    protected virtual async void ShowForm(TItem model = null)
    {
        var action = model == null || model.IsNew ? "新增" : "编辑";
        model ??= await GetDefaultModelAsync();
        ShowForm<TForm>($"{action}{Name}", model);
    }

    public override void View(TItem row) => UI.ShowForm<TForm>($"查看{Name}", row);
}