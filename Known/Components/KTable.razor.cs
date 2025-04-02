using AntDesign;
using AntDesign.TableModels;

namespace Known.Components;

/// <summary>
/// 表格组件。
/// </summary>
/// <typeparam name="TItem">表格行数据类型。</typeparam>
partial class KTable<TItem> : BaseComponent
{
    private AntTable<TItem> table;
    private int totalCount;
    private List<TItem> dataSource;
    private bool isQuering = false;
    private string ScrollY => Model.FixedHeight ?? "800px";

    /// <summary>
    /// 取得或设置表格数据模型。
    /// </summary>
    [Parameter] public TableModel<TItem> Model { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Model.OnStateChanged = StateChanged;
        Model.OnStateChangedTask = StateChangedAsync;
        Model.OnRefresh = RefreshTableAsync;
        base.OnInitialized();
    }

    private Task RefreshTableAsync(bool isQuery)
    {
        Model.Criteria.IsQuery = isQuery;
        var query = table?.GetQueryModel();
        return OnChange(query);
    }

    private async Task OnChange(QueryModel query)
    {
        if (Model.OnQuery == null || isQuering)
            return;

        try
        {
            isQuering = true;
            var watch = Stopwatcher.Start<TItem>();
            Model.Criteria.PageIndex = query.PageIndex;
            if (Model.Criteria.IsQuery)
                Model.Criteria.PageIndex = 1;
            Model.Criteria.PageSize = query.PageSize;
            if (query.SortModel != null)
            {
                //var sorts = query.SortModel.Where(s => !string.IsNullOrWhiteSpace(s.Sort));
                var sorts = query.SortModel.Where(s => s.SortDirection != SortDirection.None);
                Model.Criteria.OrderBys = sorts.Select(GetOrderBy).ToArray();
            }
            Model.Criteria.StatisticColumns = Model.Columns.Where(c => c.IsSum).Select(c => new StatisticColumnInfo { Id = c.Id }).ToList();
            Model.SelectedRows = [];
            Model.Result = await Model.OnQuery?.Invoke(Model.Criteria);
            totalCount = Model.Result.TotalCount;
            dataSource = Model.Result.PageData;
            Model.SetAutoColumns(dataSource);
            await StateChangedAsync();
            await Model.RefreshStatisAsync();
            Model.Criteria.IsQuery = false;
            isQuering = false;
            watch.Write($"Changed {Model.Criteria.PageIndex}");
            //Console.WriteLine($"{typeof(TItem)}-OnChange-{totalCount}");
        }
        catch (Exception ex)
        {
            isQuering = false;
            App?.OnErrorAsync(ex);
        }
    }

    private Dictionary<string, object> OnRow(RowData<TItem> row)
    {
        var attributes = new Dictionary<string, object>();
        if (Model.OnRowClick != null)
            attributes["onclick"] = this.Callback(async () => await Model.OnRowClick.Invoke(row.Data));

        if (Model.OnRowDoubleClick != null)
            attributes["ondblclick"] = this.Callback(async () => await Model.OnRowDoubleClick.Invoke(row.Data));

        return attributes;
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
        return text;
    }

    private static string GetActionColor(string style)
    {
        return style == "danger" ? "red-inverse" : "blue-inverse";
    }

    private void OnAddColumn()
    {
        Plugin?.AddTableColumn();
    }

    private void OnAddAction()
    {
        Plugin?.AddTableAction();
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
}