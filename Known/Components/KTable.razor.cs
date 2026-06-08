using AntDesign;
using AntDesign.Filters;

namespace Known.Components;

/// <summary>
/// 表格组件类。
/// </summary>
/// <typeparam name="TItem">表格行数据类型。</typeparam>
partial class KTable<TItem>
{
    private static readonly ConcurrentDictionary<string, Func<TItem, object>> getterCache = new(StringComparer.OrdinalIgnoreCase);
    private ReloadContainer reload;
    private AntTable<TItem> table;
    private int totalCount;
    private List<TItem> dataSource;
    private bool isRefreshing = false;
    private bool isQuering = false;
    private bool shouldRender = true;
    private int columnStateSignature = int.MinValue;
    private List<ColumnRenderState> columnStates;

    private string ScrollX => Model.IsScroll ? Model.TotalWidth : null;
    private string ScrollY => Model.IsScroll ? Model.FixedHeight : null;

    /// <summary>
    /// 取得或设置表格数据模型。
    /// </summary>
    [Parameter] public TableModel<TItem> Model { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Model.Table = this;
        Model.OnStateChanged = StateChanged;
        Model.OnStateChangedTask = StateChangedAsync;
        Model.OnRefresh = RefreshTableAsync;
        Model.OnReload = () => reload?.Reload();
        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override Task OnParameterAsync()
    {
        InvalidateColumnStates();
        return base.OnParameterAsync();
    }

    /// <inheritdoc />
    protected override bool ShouldRender()
    {
        return shouldRender;
    }

    /// <inheritdoc />
    protected override async Task OnRenderAsync(bool firstRender)
    {
        //var watch = Stopwatcher.Start<TItem>();
        await base.OnRenderAsync(firstRender);
        //if (firstRender)
        shouldRender = false;
        //watch.Write($"AfterRender-{firstRender}");
        //Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {typeof(TItem).Name}：AfterRender-{firstRender}");
    }

    internal void Select(TItem item)
    {
        Model.SelectedRows = [item];
        table.SetSelection(item);
    }

    private async Task RefreshTableAsync(bool isQuery)
    {
        isRefreshing = true;
        Model.Criteria.IsQuery = isQuery;
        var query = table?.GetQueryModel();
        await InvokeAsync(async () =>
        {
            await OnChange(query);
            StateHasChanged();
        });
        isRefreshing = false;
    }

    private async Task OnChange(QueryModel query)
    {
        if (!Model.IsAutoLoad && !isRefreshing)
            return;

        if (Model.OnQuery == null || isQuering)
            return;

        try
        {
            var watch = Stopwatcher.Start<TItem>();
            isQuering = true;
            Model.Criteria.PageIndex = query.PageIndex;
            if (Model.Criteria.IsQuery)
                Model.Criteria.PageIndex = 1;
            Model.Criteria.PageSize = query.PageSize;
            if (query.SortModel != null)
            {
                //var sorts = query.SortModel.Where(s => !string.IsNullOrWhiteSpace(s.Sort));
                var sorts = query.SortModel.Where(s => s.SortDirection != SortDirection.None);
                Model.Criteria.OrderBys = [.. sorts.Select(GetOrderBy)];
            }
            Model.Criteria.StatisticColumns = [.. Model.Columns.Where(c => c.IsSum).Select(c => new StatisticColumnInfo { Id = c.Id })];
            Model.Result = await Model.OnQuery.Invoke(Model.Criteria);
            watch.Write($"Query{Model.Criteria.PageIndex}");

            if (!string.IsNullOrWhiteSpace(Model.Result.Message))
            {
                UI.Error(Model.Result.Message);
                return;
            }

            shouldRender = true;
            totalCount = Model.Result.TotalCount;
            dataSource = Model.Result.PageData;
            Model.SelectedRows = [];
            Model.SetAutoColumns(dataSource);
            InvalidateColumnStates();
            await Model.RefreshStatisAsync();
            Model.Criteria.IsQuery = false;
            isQuering = false;
            watch.Write($"Changed{Model.Criteria.PageIndex}");
        }
        catch (Exception ex)
        {
            isQuering = false;
            _ = OnErrorAsync(ex);
        }
    }

    private Dictionary<string, object> OnRow(RowData<TItem> row)
    {
        var attributes = new Dictionary<string, object>();
        if (Model.OnRowClick != null)
        {
            attributes["onclick"] = this.Callback(async () =>
            {
                Model.SelectedRows = [row.Data];
                await Model.OnRowClick.Invoke(row.Data);
            });
        }

        if (Model.OnRowDoubleClick != null)
            attributes["ondblclick"] = this.Callback(async () => await Model.OnRowDoubleClick.Invoke(row.Data));

        return attributes;
    }

    private void OnViewForm(TItem row)
    {
        shouldRender = false;
        Model.ViewForm(row);
    }

    private void OnLinkAction(ColumnInfo item, TItem row)
    {
        shouldRender = false;
        item.LinkAction.Invoke(row);
    }

    private Task OnActionClick(ActionInfo item, TItem row)
    {
        shouldRender = false;
        Model.OnAction?.Invoke(item, row);
        return Task.CompletedTask;
    }

    private List<ColumnInfo> GetColumns()
    {
        var columns = new List<ColumnInfo>();
        var lefts = Model.Columns.Where(c => c.IsVisible && c.Fixed == "left").OrderBy(c => c.Sort).ToList();
        if (lefts != null && lefts.Count > 0)
            columns.AddRange(lefts);
        var items = Model.Columns.Where(c => c.IsVisible && c.Fixed != "left" && c.Fixed != "right").OrderBy(c => c.Sort).ToList();
        if (items != null && items.Count > 0)
            columns.AddRange(items);
        var rights = Model.Columns.Where(c => c.IsVisible && c.Fixed == "right").OrderBy(c => c.Sort).ToList();
        if (rights != null && rights.Count > 0)
            columns.AddRange(rights);
        return columns;
    }

    private List<ColumnRenderState> GetColumnStates()
    {
        var signature = GetColumnStateSignature();
        if (signature == columnStateSignature && columnStates != null)
            return columnStates;

        var columns = GetColumns();
        columnStates = [.. columns
            .Where(c => !string.IsNullOrWhiteSpace(c.Id))
            .Select(CreateColumnState)];
        columnStateSignature = signature;
        return columnStates;
    }

    private int GetIndex(TItem item)
    {
        var seqNo = Model.Result.PageData.IndexOf(item) + 1;
        if (!Model.ShowPager)
            return seqNo;

        return seqNo + (Model.Criteria.PageIndex - 1) * Model.Criteria.PageSize;
    }

    private bool GetSortable(ColumnInfo item)
    {
        if (!Model.EnableSort)
            return false;

        return item.IsSort;
    }

    private string GetOrderBy(ITableSortModel model)
    {
        //descend  ascend
        //var sort = model.Sort == "descend" ? "desc" : "asc";
        var sort = model.SortDirection == SortDirection.Descending ? "desc" : "asc";
        var fieldName = model.FieldName;
        if (string.IsNullOrWhiteSpace(fieldName) && model.ColumnIndex > 0)
        {
            if (Model.Columns.Count >= model.ColumnIndex - 1)
                fieldName = Model.Columns[model.ColumnIndex - 1].Id;
        }

        if (string.IsNullOrWhiteSpace(fieldName))
            return string.Empty;

        return $"{fieldName} {sort}";
    }

    private static SelectionType GetSelectionType(TableSelectType type)
    {
        //return type.ToString().ToLower();
        return type switch
        {
            TableSelectType.Checkbox => SelectionType.Checkbox,
            TableSelectType.Radio => SelectionType.Radio,
            _ => SelectionType.Checkbox
        };
    }

    private static string GetColumnText(ColumnInfo item, object value)
    {
        var text = $"{value}";
        if (item.Type == FieldType.Date)
        {
            if (value is string)
                value = Utils.ConvertTo<DateTime?>(value);
            text = $"{value:yyyy-MM-dd}";
        }
        if (item.Type == FieldType.DateTime)
        {
            if (value is string)
                value = Utils.ConvertTo<DateTime?>(value);
            text = $"{value:yyyy-MM-dd HH:mm:ss}";
        }
        else if (!string.IsNullOrWhiteSpace(item.Category))
        {
            if (value is string[] values)
                text = Cache.GetCodeName(item.Category, values);
            else
                text = Cache.GetCodeName(item.Category, text);
        }
        if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(item.Unit))
            text += $" {item.Unit}";
        return text;
    }

    private void InvalidateColumnStates()
    {
        columnStateSignature = int.MinValue;
        columnStates = null;
    }

    private int GetColumnStateSignature()
    {
        var hash = new HashCode();
        hash.Add(Model?.EnableFilter ?? false);
        hash.Add(Model?.EnableSort ?? false);
        hash.Add(UIConfig.IsEllipsisTable);
        hash.Add(System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(Language));

        if (Model?.Columns == null)
            return hash.ToHashCode();

        foreach (var item in Model.Columns)
        {
            hash.Add(item.Id, StringComparer.OrdinalIgnoreCase);
            hash.Add(item.IsVisible);
            hash.Add(item.Sort);
            hash.Add(item.Width ?? 0);
            hash.Add(item.Fixed, StringComparer.OrdinalIgnoreCase);
            hash.Add(item.Align, StringComparer.OrdinalIgnoreCase);
            hash.Add(item.DefaultSort, StringComparer.OrdinalIgnoreCase);
            hash.Add(item.Ellipsis);
            hash.Add(item.IsSort);
            hash.Add(item.IsSum);
            hash.Add(item.IsQueryField);
            hash.Add(item.IsFilter);
            hash.Add(item.IsViewLink);
            hash.Add(item.Type);
            hash.Add(item.Category, StringComparer.OrdinalIgnoreCase);
            hash.Add(item.Tooltip, StringComparer.OrdinalIgnoreCase);
            hash.Add(item.Unit, StringComparer.OrdinalIgnoreCase);
            hash.Add(Model.Templates?.ContainsKey(item.Id) == true);
        }

        return hash.ToHashCode();
    }

    private ColumnRenderState CreateColumnState(ColumnInfo item)
    {
        RenderFragment<TItem> template = null;
        Model.Templates?.TryGetValue(item.Id, out template);
        var hasTemplate = template != null;
        var hasCustomText = item.Type == FieldType.Date || item.Type == FieldType.DateTime || !string.IsNullOrWhiteSpace(item.Category) || !string.IsNullOrWhiteSpace(item.Unit);
        var hasCustomCell = hasTemplate || item.Type == FieldType.Switch || item.Type == FieldType.File || item.IsViewLink || item.LinkAction != null || item.IsMergeRow || item.IsMergeColumn;
        return new ColumnRenderState
        {
            Column = item,
            Title = Language?.GetFieldName<TItem>(item),
            IsEllipsis = item.ToEllipsis(),
            Width = item.Width > 0 ? item.Width.ToString() : string.Empty,
            Fixed = item.ToColumnFixPlacement(),
            Align = item.ToColumnAlign(),
            Sortable = GetSortable(item),
            DefaultSortOrder = item.ToSortDirection(),
            Template = template,
            FilterDropdown = GetFilterTemplate(item),
            ValueGetter = Model.IsDictionary ? null : GetValueGetter(item),
            UseDefaultCellRender = !Model.IsDictionary && !hasCustomCell && !hasCustomText
        };
    }

    private static Func<TItem, object> GetValueGetter(ColumnInfo item)
    {
        if (item == null || string.IsNullOrWhiteSpace(item.Id))
            return _ => null;

        return getterCache.GetOrAdd(item.Id, _ => CreateValueGetter(item));
    }

    private static Func<TItem, object> CreateValueGetter(ColumnInfo item)
    {
        var property = item.Property ?? TypeHelper.Property(typeof(TItem), item.Id);
        if (property == null)
            return row => TypeHelper.GetPropertyValue(row, item.Id);

        var instance = Expression.Parameter(typeof(TItem), "instance");
        Expression target = instance;
        if (property.DeclaringType != typeof(TItem))
            target = Expression.Convert(instance, property.DeclaringType!);
        var propertyAccess = Expression.Property(target, property);
        var castPropertyValue = Expression.Convert(propertyAccess, typeof(object));
        return Expression.Lambda<Func<TItem, object>>(castPropertyValue, instance).Compile();
    }

    //private readonly List<string> mergeRows = [];
    private int GetRowSpan(ColumnInfo item, object value)
    {
        if (!item.IsMergeRow || dataSource == null || value == null)
            return 1;

        //if (mergeRows.Contains(item.Id))
        //    return 0;

        //mergeRows.Add(item.Id);
        //var span = dataSource.Count(d => d.Property(item.Id)?.Equals(value) == true);
        //return span;
        return 1;
    }

    private int GetRowSpan(ColumnRenderState state, TItem row)
    {
        if (state?.Column == null)
            return 1;

        if (!state.Column.IsMergeRow)
            return 1;

        var value = state.GetValue(row);
        return GetRowSpan(state.Column, value);
    }

    private int GetColSpan(ColumnInfo item, object value)
    {
        if (!item.IsMergeColumn || dataSource == null || value == null)
            return 1;

        return 1;
    }

    private int GetColSpan(ColumnRenderState state, TItem row)
    {
        if (state?.Column == null)
            return 1;

        if (!state.Column.IsMergeColumn)
            return 1;

        var value = state.GetValue(row);
        return GetColSpan(state.Column, value);
    }

    private void OnAddColumn()
    {
        Plugin?.AddTableColumn();
    }

    private void OnAddAction()
    {
        Plugin?.AddTableAction();
    }

    private RenderFragment<TableFilterDropdownContext> GetFilterTemplate(ColumnInfo item)
    {
        if (!Model.EnableFilter || !item.IsQueryField || !item.IsFilter)
            return null;

        if (item.FilterTemplate != null)
            return this.BuildTree<TableFilterDropdownContext>((b, r) => b.Fragment(item.FilterTemplate));

        return this.BuildTree<TableFilterDropdownContext>(
            (b, r) => b.Component<Internals.TableFilter<TItem>>()
                       .Set(c => c.Table, Model)
                       .Set(c => c.Item, item)
                       .Build()
        );
    }

    private RenderFragment<TItem> HeaderTemplate
    {
        get
        {
            if (Model.HeaderTemplate == null)
                return null;

            return this.BuildTree<TItem>((b, d) => Model.HeaderTemplate(b, d));
        }
    }

    private RenderFragment<RowData<TItem>> ExpandTemplate
    {
        get
        {
            if (Model.ExpandTemplate == null)
                return null;

            return this.BuildTree<RowData<TItem>>((b, d) => Model.ExpandTemplate(b, d.Data));
        }
    }

    private EventCallback<RowData<TItem>> OnExpand
    {
        get
        {
            if (Model.OnExpand.HasDelegate)
                return Model.OnExpand;

            return this.Callback<RowData<TItem>>(e => { });
        }
    }

    private sealed class ColumnRenderState
    {
        public ColumnInfo Column { get; init; }
        public string Title { get; init; }
        public bool IsEllipsis { get; init; }
        public string Width { get; init; }
        public ColumnFixPlacement? Fixed { get; init; }
        public ColumnAlign Align { get; init; }
        public bool Sortable { get; init; }
        public SortDirection DefaultSortOrder { get; init; }
        public RenderFragment<TItem> Template { get; init; }
        public RenderFragment<TableFilterDropdownContext> FilterDropdown { get; init; }
        public Func<TItem, object> ValueGetter { get; init; }
        public bool UseDefaultCellRender { get; init; }

        public object GetValue(TItem item) => ValueGetter?.Invoke(item);
    }
}
