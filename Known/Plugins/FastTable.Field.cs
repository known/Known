namespace Known.Plugins;

/// <summary>
/// 快速选择字段表格组件类。
/// </summary>
public class FastFieldTable : BaseTable<FieldDataInfo>, IFastTable<FieldDataInfo>
{
    private IFieldService Service;

    /// <summary>
    /// 取得或设置查询参数。
    /// </summary>
    [Parameter] public string Query { get; set; }

    /// <summary>
    /// 取得或设置已选择的数据列表。
    /// </summary>
    [Parameter] public List<string> SelectedItems { get; set; }

    /// <summary>
    /// 取得或设置表格行双击事件委托。
    /// </summary>
    [Parameter] public Func<FieldDataInfo, Task> OnRowDoubleClick { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IFieldService>();

        Table.ShowPager = true;
        Table.FixedHeight = "300px";
        Table.SelectType = TableSelectType.Checkbox;
        Table.OnQuery = OnDataQueryAsync;

        Table.AddColumn(c => c.Code);
        Table.AddColumn(c => c.Name, true);
        Table.AddColumn(c => c.Type).Tag();
        Table.AddColumn(c => c.Length);
    }

    private Task<PagingResult<FieldDataInfo>> OnDataQueryAsync(PagingCriteria criteria)
    {
        criteria.SetQuery(nameof(FieldDataInfo.Code), QueryType.NotIn, SelectedItems);
        return Service.QueryFieldsAsync(criteria);
    }
}