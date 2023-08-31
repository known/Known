namespace Known.Razor.Pages.Forms;

class SysMyTodoList : DataGrid<SysFlow>
{
    public SysMyTodoList()
    {
        IsSort = false;

        var builder = new ColumnBuilder<SysFlow>();
        builder.Field(r => r.FlowName).Center(100);
        builder.Field(r => r.BizStatus).Center(100).Template(BuildBizStatus);
        builder.Field(r => r.BizName, true);
        builder.Field(r => r.ApplyTime).Type(ColumnType.DateTime);
        Columns = builder.ToColumns();
    }

    protected override async Task InitPageAsync()
    {
        Data = await Platform.Flow.GetFlowTodosAsync();
        await base.InitPageAsync();
    }

    private void BuildBizStatus(RenderTreeBuilder builder, SysFlow flow)
    {
        builder.StatusTag(flow.BizStatus);
    }
}