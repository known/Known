namespace Known.Razor.Pages.Forms;

class SysMyFlowList : DataGrid<SysFlow>, IMyFlow
{
    public SysMyFlowList()
    {
        IsSort = false;
        ShowPager = false;

        var builder = new ColumnBuilder<SysFlow>();
        builder.Field(r => r.FlowName).Center(100);
        builder.Field(r => r.BizStatus).Center(100).Template(BuildBizStatus);
        builder.Field(r => r.BizName).Template(BuildBizName);
        builder.Field(r => r.ApplyTime).Type(ColumnType.DateTime);
        Columns = builder.ToColumns();
    }

    public SysFlow Flow { get; set; }

    public override async void Refresh()
    {
        Data = await Platform.Flow.GetFlowTodosAsync();
        StateChanged();
    }

    protected override async Task InitPageAsync()
    {
        Data = await Platform.Flow.GetFlowTodosAsync();
        await base.InitPageAsync();
    }

    private void BuildBizStatus(RenderTreeBuilder builder, SysFlow row)
    {
        builder.StatusTag(row.BizStatus);
    }

    private void BuildBizName(RenderTreeBuilder builder, SysFlow row)
    {
        builder.Link(row.BizName, Callback(() => OnBizNameClick(row)));
    }

    private void OnBizNameClick(SysFlow row)
    {
        Flow = row;
        KRConfig.ShowMyFlow?.Invoke(this);
    }
}