namespace Known.Razor.Pages;

public class FlowLogGrid : DataGrid<SysFlowLog>
{
    public FlowLogGrid()
    {
        IsSort = false;
        ReadOnly = true;
        ShowEmpty = false;
        ShowPager = false;

        var builder = new ColumnBuilder<SysFlowLog>();
        builder.Field(r => r.StepName).Center();
        builder.Field(r => r.ExecuteBy).Center();
        builder.Field(r => r.ExecuteTime).Type(ColumnType.DateTime);
        builder.Field(r => r.Result).Center();
        builder.Field(r => r.Note);
        Columns = builder.ToColumns();
    }

    [Parameter] public string BizId { get; set; }
    [Parameter] public bool IsFluent { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        if (IsFluent)
        {
            ContainerStyle = "";
            ContentStyle = "";
        }

        await base.OnInitializedAsync();
        Data = await Platform.Flow.GetFlowLogsAsync(BizId);
    }
}