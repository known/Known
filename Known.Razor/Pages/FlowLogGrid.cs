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
        builder.Field(r => r.StepName).Center(80);
        builder.Field(r => r.ExecuteBy).Center(80);
        builder.Field(r => r.ExecuteTime).Center(150).Type(ColumnType.DateTime);
        builder.Field(r => r.Result).Center(80).Template(BuildResult);
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

    private void BuildResult(RenderTreeBuilder builder, SysFlowLog log)
    {
        if (string.IsNullOrWhiteSpace(log.Result))
            return;

        var style = StyleType.Default;
        if (log.Result == Language.Submit)
            style = StyleType.Info;
        else if (log.Result == "分配" || log.Result == "重启")
            style = StyleType.Primary;
        else if (log.Result == "终止" || log.Result == Language.Return || log.Result == Language.Revoke)
            style = StyleType.Danger;
        else if (log.Result == "结束" || log.Result == Language.Pass)
            style = StyleType.Success;
        builder.Tag(style, log.Result);
    }
}