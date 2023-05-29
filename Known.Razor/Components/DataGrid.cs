using Known.Razor.Pages.Forms;

namespace Known.Razor.Components;

public class DataGrid<TItem> : DataComponent<TItem>
{
    private bool checkAll = false;
    private readonly string id;
    private List<Column<TItem>> gridColumns;
    protected int curRow = -1;

    public DataGrid()
    {
        id = Utils.GetGuid();
        ContainerStyle = "grid-view box";
        ContentStyle = "grid";
        InitMenu();
    }

    protected List<Column<TItem>> Columns { get; set; }
    protected List<ButtonInfo> Actions { get; set; }
    protected Action<RenderTreeBuilder> ActionHead { get; set; }
    protected bool IsEdit { get; set; }
    protected bool IsFixed { get; set; } = true;
    protected bool IsSort { get; set; } = true;
    protected bool ShowEmpty { get; set; } = true;
    protected bool ShowSetting { get; set; }
    protected bool ShowCheckBox { get; set; }
    protected string OrderBy { get; set; }
    protected string RowTitle { get; set; }

    public override void Refresh()
    {
        query = null;
        QueryData();
    }

    protected void SetEdit(bool isEdit)
    {
        IsEdit = isEdit;
        foreach (var item in gridColumns)
        {
            item.IsEdit = isEdit;
        }
        StateChanged();
    }

    public void SetColumn(string id, bool isVisible)
    {
        if (gridColumns == null || gridColumns.Count == 0)
            return;

        var column = gridColumns.FirstOrDefault(c => c.Id == id);
        if (column == null)
            return;

        column.IsVisible = isVisible;
        StateChanged();
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

    protected void SelectItem(Action<TItem> action)
    {
        var selected = SelectedItems;
        if (!selected.Any() || selected.Count > 1)
        {
            UI.Tips(Language.SelectOne);
            return;
        }

        action.Invoke(selected[0]);
    }

    protected void SelectItems(Action<List<TItem>> action)
    {
        var selected = SelectedItems;
        if (!selected.Any())
        {
            UI.Tips(Language.SelectOneAtLeast);
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
        SelectItems(items =>
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
        ShowImport(new ImportOption { Id = id, Name = name, Model = model });
    }

    internal void ShowImport(ImportOption option)
    {
        UI.Show<Importer>($"导入{option.Name}", new Size(450, 220), action: attr => attr.Set(c => c.Option, option));
    }

    internal async void ExportData(string name, ExportMode mode, string extension = null)
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

    protected override async Task OnInitializedAsync()
    {
        FormatColumns();
        gridColumns = Setting.GetUserColumns(Id, Columns);
        ShowQuery = gridColumns != null && gridColumns.Any(c => c.IsQuery);

        if (!string.IsNullOrWhiteSpace(OrderBy))
        {
            OrderBys = new string[] { OrderBy };
            var orders = OrderBy.Split(' ');
            curId = orders[0];
            if (orders.Length > 1)
                curOrder = orders[1];
        }

        await AddVisitLogAsync();
        await base.OnInitializedAsync();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (IsFixed)
            UI.FixedTable(id);

        return base.OnAfterRenderAsync(firstRender);
    }

    protected override void BuildContent(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("table").AddClass("fixed", IsFixed).Build();
        builder.Div(css, attr =>
        {
            if (Data == null || Data.Count == 0)
            {
                BuildTable(builder);
                if (ShowEmpty) BuildEmpty(builder);
            }
            else
            {
                BuildTable(builder, () =>
                {
                    var index = 0;
                    foreach (TItem item in Data)
                    {
                        BuildRowItem(builder, index, item);
                        index++;
                    }
                });
            }
        });
    }

    protected override void BuildQuerys(RenderTreeBuilder builder)
    {
        var columns = gridColumns?.Where(c => c.IsQuery).ToList();
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

        if (gridColumns != null && gridColumns.Any(c => c.IsAdvQuery))
        {
            builder.Button(FormButton.AdvQuery, Callback(ShowQuerySetting));
        }
    }

    protected virtual void BuildHead(RenderTreeBuilder builder)
    {
        builder.THead(attr =>
        {
            builder.Tr(attr =>
            {
                BuildHeadIndex(builder);
                BuildHeadCheckBox(builder);
                
                if (gridColumns != null && gridColumns.Count > 0)
                {
                    foreach (var item in gridColumns)
                    {
                        if (!item.IsVisible)
                            continue;

                        builder.Th(item.ClassName, attr =>
                        {
                            var canSort = IsSort && item.IsSort && !string.IsNullOrWhiteSpace(item.Id);
                            if (item.Width > 0)
                                attr.Style($"width:{item.Width}px;");
                            if (canSort)
                                attr.OnClick(Callback(() => OnSort(item)));
                            builder.Text(item.Name);
                            if (canSort)
                                SetSortIcon(builder, item);
                        });
                    }
                }

                BuildHeadAction(builder);
            });
        });
    }

    protected void SetColumns(List<Column<TItem>> columns)
    {
        gridColumns = columns;
        ShowQuery = gridColumns != null && gridColumns.Any(c => c.IsQuery);
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
        if (gridColumns == null || gridColumns.Count == 0)
            return null;

        return gridColumns.Where(c => c.IsSum).Select(c => c.Id).ToList();
    }

    public virtual void View(TItem item) { }
    public virtual void Import() => ShowImport(Name, typeof(TItem));
    public virtual void Export() => ExportData(Name, ExportMode.Query);

    protected virtual void FormatColumns() { }
    protected virtual bool CheckAction(ButtonInfo action, TItem item) => true;
    protected virtual void OnRowClick(int row, TItem item) { }
    protected virtual void OnRowDoubleClick(int row, TItem item) { }
    protected void OnDelete(TItem item, Func<List<TItem>, Task<Result>> action) => DeleteRow(item, action);
    protected virtual void OnDeleteM(Func<List<TItem>, Task<Result>> action) => DeleteRows(action);
    protected void OnExport(ExportMode mode, string extension = null) => ExportData(Name, mode, extension);

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

    private void BuildTable(RenderTreeBuilder builder, Action action = null)
    {
        builder.Table(attr =>
        {
            attr.Id(id);
            BuildHead(builder);
            builder.TBody(attr => action?.Invoke());
            BuildFoot(builder);
        });
    }

    private void BuildHeadIndex(RenderTreeBuilder builder)
    {
        builder.Th("fixed index", attr =>
        {
            if (ShowSetting)
            {
                builder.Icon("fa fa-cog", attr => attr.Title("表格设置").OnClick(Callback(ShowColumnSetting)));
            }
        });
    }

    private void BuildHeadCheckBox(RenderTreeBuilder builder)
    {
        if (!ShowCheckBox)
            return;

        builder.Th("fixed check", attr =>
        {
            builder.Label("form-radio", attr =>
            {
                builder.Check(attr => attr.Checked(checkAll).OnClick(Callback(() =>
                {
                    checkAll = !checkAll;
                    SelectedItems.Clear();
                    if (checkAll && Data != null && Data.Count > 0)
                        SelectedItems.AddRange(Data);
                })));
                builder.Span("");
            });
        });
    }

    private void BuildHeadAction(RenderTreeBuilder builder)
    {
        if (IsEdit && ReadOnly)
            return;

        if (Actions == null || Actions.Count == 0)
            return;

        builder.Th("action", attr =>
        {
            if (ActionHead != null)
                ActionHead.Invoke(builder);
            else
                builder.Text("操作");
        });
    }

    private void BuildRowItem(RenderTreeBuilder builder, int index, TItem item)
    {
        var rowNo = index + 1;
        var css = CssBuilder.Default("")
                            .AddClass("even", rowNo % 2 == 0)
                            .AddClass("active", curRow == index)
                            .AddClass("selected", SelectedItems.Contains(item))
                            .Build();
        builder.Tr(css, attr =>
        {
            attr.Title(RowTitle);
            var context = new FieldContext { Model = item };
            attr.OnClick(Callback(() =>
            {
                OnRowClick(rowNo, item);
                curRow = Data.IndexOf(item);
                StateChanged();
            }));

            attr.OnDoubleClick(Callback(() => OnRowDoubleClick(rowNo, item)));
            BuildRowIndex(builder, rowNo);
            BuildRowCheckBox(builder, rowNo, item);
            
            if (IsEdit)
            {
                builder.Component((Action<AttributeBuilder<CascadingValue<FieldContext>>>)(attr =>
                {
                    attr.Set(c => c.IsFixed, false)
                        .Set(c => c.Value, context)
                        .Set(c => c.ChildContent, BuildTree<TItem>(BuildRowContent).Invoke(item));
                }));
            }
            else
            {
                BuildRowContent(builder, item);
            }

            BuildRowAction(builder, rowNo, item);
        });
    }

    private void BuildRowContent(RenderTreeBuilder builder, TItem item)
    {
        if (gridColumns == null || gridColumns.Count == 0)
            return;

        var data = Utils.MapTo<Dictionary<string, object>>(item);
        foreach (var column in gridColumns)
        {
            if (!column.IsVisible)
                continue;

            builder.Td(column.ClassName, attr =>
            {
                var value = data != null && data.ContainsKey(column.Id)
                          ? data[column.Id]
                          : null;
                column.BuildCell(builder, item, value, ReadOnly);
            });
        }
    }

    private static void BuildRowIndex(RenderTreeBuilder builder, int index)
    {
        builder.Td("fixed index", attr => builder.Text($"{index}"));
    }

    private void BuildRowCheckBox(RenderTreeBuilder builder, int index, TItem item)
    {
        if (!ShowCheckBox)
            return;

        builder.Td("fixed check", attr =>
        {
            builder.Label("form-radio", attr =>
            {
                builder.Check(attr =>
                {
                    attr.Checked(SelectedItems.Contains(item)).OnClick(Callback(e =>
                    {
                        if (SelectedItems.Contains(item))
                            SelectedItems.Remove(item);
                        else
                            SelectedItems.Add(item);
                        checkAll = SelectedItems.Count == TotalCount ||
                                   SelectedItems.Count == criteria.PageSize;
                    }));
                });
                builder.Span("");
            });
        });
    }

    private void BuildRowAction(RenderTreeBuilder builder, int index, TItem item)
    {
        if (IsEdit && ReadOnly)
            return;

        if (Actions == null || Actions.Count == 0)
            return;

        builder.Td("action", attr =>
        {
            var count = 0;
            var actions = new List<ButtonInfo>();
            var others = new List<ButtonInfo>();
            foreach (var action in Actions)
            {
                if (!CheckAction(action, item))
                    continue;

                if (count++ < 2 || Actions.Count == 3)
                    actions.Add(action);
                else
                    others.Add(action);
            }
            BuildRowAction(builder, item, actions);
            BuildRowMoreAction(builder, index, item, others);
        });
    }

    private void BuildRowAction(RenderTreeBuilder builder, TItem item, List<ButtonInfo> actions)
    {
        foreach (var action in actions)
        {
            builder.Link(action.Name, Callback(() => OnRowAction(item, action)), action.Style);
        }
    }

    private void BuildRowMoreAction(RenderTreeBuilder builder, int index, TItem item, List<ButtonInfo> others)
    {
        if (others.Count == 0)
            return;

        builder.Div("dropdown", attr =>
        {
            builder.Span("link bg-primary", attr =>
            {
                builder.Text("更多");
                builder.Icon("fa fa-caret-down");
            });
            builder.Ul("child box", attr =>
            {
                foreach (var action in others)
                {
                    builder.Li("item", attr =>
                    {
                        attr.OnClick(Callback(() => OnRowAction(item, action)));
                        builder.Span("", action.Name);
                    });
                }
            });
        });
    }

    private void OnRowAction(TItem item, ButtonInfo action)
    {
        var method = GetType().GetMethod(action.Id);
        if (method == null)
            UI.Tips($"{action.Name}方法不存在！");
        else
            method.Invoke(this, new object[] { item });
    }

    private void BuildFoot(RenderTreeBuilder builder)
    {
        if (!HasFoot())
            return;

        builder.TFoot(attr =>
        {
            builder.Tr(attr =>
            {
                var colSpan = 1;
                if (ShowCheckBox) colSpan++;
                //if (Actions != null && Actions.Count > 0) colSpan++;
                if (gridColumns != null && gridColumns.Count > 0)
                {
                    builder.Td("index", "合计", colSpan);
                    foreach (var item in gridColumns)
                    {
                        if (!item.IsVisible)
                            continue;

                        builder.Td(attr =>
                        {
                            if (item.IsSum)
                            {
                                attr.Class("txt-right");
                                var value = Sums != null && Sums.ContainsKey(item.Id)
                                          ? Sums[item.Id]
                                          : 0;
                                builder.Text($"{value}");
                            }
                        });
                    }
                }
            });
        });
    }

    private bool HasFoot()
    {
        if (gridColumns == null || gridColumns.Count == 0)
            return false;

        var columns = gridColumns.Where(c => c.IsSum).ToList();
        if (columns != null && columns.Count > 0)
            return true;

        return false;
    }

    private void OnMoveRow(TItem item, Action<TItem, TItem> success, int index, int index1)
    {
        var temp = Data[index1];
        Data[index1] = item;
        Data[index] = temp;
        success?.Invoke(item, temp);
        curRow = index1;
        StateChanged();
    }

    private string curId = "";
    private string curOrder = "asc";
    private void OnSort(ColumnInfo item)
    {
        curId = item.Id;
        curOrder = curOrder == "asc" ? "desc" : "asc";
        OrderBys = new string[] { $"{item.Id} {curOrder}" };
        QueryData();
    }

    private void SetSortIcon(RenderTreeBuilder builder, ColumnInfo item)
    {
        if (curId == item.Id)
            builder.Icon($"sort fa fa-sort-{curOrder}");
        else
            builder.Icon("fa fa-sort");
    }

    private void ShowQuerySetting()
    {
        var data = Setting.GetUserQuerys(Id);
        data ??= new List<QueryInfo>();
        var fields = Columns.Select(c => c.ToColumn()).ToList();
        UI.Show<QueryGrid>("高级查询", new(680, 500), action: attr =>
        {
            attr.Set(c => c.Data, data)
                .Set(c => c.Fields, fields)
                .Set(c => c.OnSetting, async value =>
                {
                    var info = new SettingFormInfo
                    {
                        Type = UserSetting.KeyQuery,
                        Name = Id,
                        Data = Utils.ToJson(value)
                    };
                    await Platform.User.SaveSettingAsync(info);
                    Setting.UserSetting.Querys[Id] = value;
                    query = value;
                    QueryData(true);
                });
        });
    }

    private void ShowColumnSetting()
    {
        var data = Setting.GetUserColumns(Id);
        data ??= Columns.Select(c => c.ToColumn()).ToList();
        UI.Show<ColumnGrid>("表格设置", new(780, 500), action: attr =>
        {
            attr.Set(c => c.Data, data)
                .Set(c => c.OnSetting, async value =>
                {
                    var info = new SettingFormInfo
                    {
                        Type = UserSetting.KeyColumn,
                        Name = Id,
                        Data = Utils.ToJson(value)
                    };
                    await Platform.User.SaveSettingAsync(info);
                    Setting.UserSetting.Columns[Id] = value;
                    var columns = Setting.GetUserColumns(Id, Columns);
                    SetColumns(columns);
                });
        });
    }

    private Dictionary<string, string> GetExportColumns()
    {
        var columns = new Dictionary<string, string>();
        if (gridColumns != null && gridColumns.Count > 0)
        {
            foreach (var item in gridColumns)
            {
                columns.Add(item.Id, item.Name);
            }
        }
        return columns;
    }
}

public class DataGrid<TItem, TForm> : DataGrid<TItem> where TItem : EntityBase where TForm : Form
{
    public DataGrid()
    {
        ShowSetting = true;
        ShowCheckBox = true;
    }

    protected virtual Task<TItem> GetDefaultModelAsync() => default;

    protected virtual async void ShowForm(TItem model = null)
    {
        var action = model == null || model.IsNew ? "新增" : "编辑";
        model ??= await GetDefaultModelAsync();
        ShowForm<TForm>($"{action}{Name}", model);
    }

    public override void View(TItem row) => UI.ShowForm<TForm>($"查看{Name}", row);
}