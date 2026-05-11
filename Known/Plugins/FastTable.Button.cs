namespace Known.Plugins;

/// <summary>
/// 快速选择按钮表格组件类。
/// </summary>
public class FastButtonTable : BaseTable<ButtonInfo>, IFastTable<ButtonInfo>
{
    private IButtonService Service;

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
    [Parameter] public Func<ButtonInfo, Task> OnRowDoubleClick { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IButtonService>();

        Table.ShowPager = true;
        Table.FixedHeight = "300px";
        Table.SelectType = TableSelectType.Checkbox;
        Table.OnQuery = OnDataQueryAsync;

        Table.AddColumn(c => c.Id);
        Table.AddColumn(c => c.Name, true);
        Table.AddColumn(c => c.Icon, true).Template((b, r) => b.IconName(r.Icon, r.Icon));
        Table.AddColumn(c => c.Style).Tag();
        Table.AddColumn(c => c.Position);
    }

    private Task<PagingResult<ButtonInfo>> OnDataQueryAsync(PagingCriteria criteria)
    {
        criteria.SetQuery(nameof(ButtonInfo.Id), QueryType.NotIn, SelectedItems);
        criteria.SetQuery(nameof(ButtonInfo.Position), Query);
        return Service.QueryButtonsAsync(criteria);
    }
}