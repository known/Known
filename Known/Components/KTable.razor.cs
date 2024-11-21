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
    private bool isQuering = false;
    private string scrollX = "";
    private string scrollY => Model.FixedHeight ?? "1000px";

    /// <summary>
    /// 取得或设置表格数据模型。
    /// </summary>
    [Parameter] public TableModel<TItem> Model { get; set; }

    /// <summary>
    /// 初始化表格组件。
    /// </summary>
    protected override void OnInitialized()
    {
        var totalWidth = Model.Columns.Select(c => c.Width > 0 ? c.Width : 100).Sum();
        if (totalWidth > 1000)
            scrollX = totalWidth.ToString();
        Model.OnStateChanged = StateChanged;
        Model.OnStateChangedTask = StateChangedAsync;
        Model.OnRefresh = RefreshTableAsync;
        base.OnInitialized();
    }

    /// <summary>
    /// 表格数据呈现后执行方法。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        await JSRuntime.FillTableHeightAsync();
    }

    private async Task RefreshTableAsync()
    {
        await InvokeAsync(() =>
        {
            var query = table?.GetQueryModel();
            table?.ReloadData(query);
        });
    }

    private async Task OnChange(QueryModel<TItem> query)
    {
        if (Model.OnQuery == null || isQuering)
            return;

        if (IsServerMode)
        {
            isQuering = true;
            await StateChangedAsync();
            await Task.Run(async () =>
            {
                await OnChangeAsync(query);
                isQuering = false;
                await StateChangedAsync();
            });
        }
        else
        {
            isQuering = true;
            await OnChangeAsync(query);
            isQuering = false;
        }
    }

    private async Task OnChangeAsync(QueryModel<TItem> query)
    {
        var watch = Stopwatcher.Start<TItem>();
        if (Model.Criteria.IsQuery)
            Model.Criteria.PageIndex = 1;
        Model.Criteria.PageIndex = query.PageIndex;
        Model.Criteria.PageSize = query.PageSize;
        if (query.SortModel != null)
        {
            var sorts = query.SortModel.Where(s => !string.IsNullOrWhiteSpace(s.Sort));
            Model.Criteria.OrderBys = sorts.Select(m => GetOrderBy(m)).ToArray();
        }
        Model.Criteria.StatisColumns = Model.Columns.Where(c => c.IsSum).Select(c => new StatisColumnInfo { Id = c.Id }).ToList();
        Model.Result = await Model.OnQuery?.Invoke(Model.Criteria);
        Model.SelectedRows = [];
        await Model.RefreshStatisAsync();
        watch.Write($"Changed {Model.Criteria.PageIndex}");
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
        var sort = model.Sort == "descend" ? "desc" : "asc";
        var fieldName = model.FieldName;
        if (string.IsNullOrWhiteSpace(fieldName))
            fieldName = Model.Columns[model.ColumnIndex - 1].Id;
        return $"{fieldName} {sort}";
    }

    private static ColumnAlign GetColumnAlign(string align)
    {
        if (align == "center")
            return ColumnAlign.Center;
        else if (align == "right")
            return ColumnAlign.Right;
        return ColumnAlign.Left;
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
        return text;
    }

    private string GetActionColor(string style)
    {
        return style == "danger" ? "red-inverse" : "blue-inverse";
    }
}