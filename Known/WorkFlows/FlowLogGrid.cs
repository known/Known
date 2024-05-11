namespace Known.WorkFlows;

public class FlowLogGrid : BaseTable<SysFlowLog>
{
    [Parameter] public string BizId { get; set; }
    [Parameter] public List<SysFlowLog> Logs { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Logs ??= await Platform.Flow.GetFlowLogsAsync(BizId);
        Table.DataSource = Logs;
        Table.AddColumn(c => c.StepName).Width(100).Template((b, r) => b.Tag(r.StepName));
        Table.AddColumn(c => c.ExecuteBy).Width(100);
        Table.AddColumn(c => c.ExecuteTime).Width(180);
        Table.AddColumn(c => c.Result).Width(100).Template((b, r) => b.Tag(r.Result));
        Table.AddColumn(c => c.Note);
    }
}