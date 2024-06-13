namespace Known.WorkFlows;

public class FlowLogGrid : BaseTable<SysFlowLog>
{
    private IFlowService flowService;

    [Parameter] public string BizId { get; set; }
    [Parameter] public List<SysFlowLog> Logs { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        flowService = await Factory.CreateAsync<IFlowService>(Context);
        Logs ??= await flowService.GetFlowLogsAsync(BizId);
        Table.DataSource = Logs;
        Table.AddColumn(c => c.StepName).Width(100).Template((b, r) => b.Tag(r.StepName));
        Table.AddColumn(c => c.ExecuteBy).Width(100);
        Table.AddColumn(c => c.ExecuteTime).Width(180);
        Table.AddColumn(c => c.Result).Width(100).Template((b, r) => b.Tag(r.Result));
        Table.AddColumn(c => c.Note);
    }

    //protected override async Task OnAfterRenderAsync(bool firstRender)
    //{
    //    await base.OnAfterRenderAsync(firstRender);
    //    if (firstRender)
    //    {
    //        Logs ??= await flowService.GetFlowLogsAsync(BizId);
    //        Table.DataSource = Logs;
    //    }
    //}
}