namespace Known.WorkFlows;

public class FlowLogGrid : BaseTable<SysFlowLog>
{
    private IFlowService Service;

    [Parameter] public string BizId { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IFlowService>();
        Table.ShowPager = true;
        Table.OnQuery = QueryFlowLogsAsync;
        Table.AddColumn(c => c.StepName).Width(100).Template((b, r) => b.Tag(r.StepName));
        Table.AddColumn(c => c.ExecuteBy).Width(100);
        Table.AddColumn(c => c.ExecuteTime).Width(180).DefaultAscend();
        Table.AddColumn(c => c.Result).Width(100).Template((b, r) => b.Tag(r.Result));
        Table.AddColumn(c => c.Note);
    }

    private Task<PagingResult<SysFlowLog>> QueryFlowLogsAsync(PagingCriteria criteria)
    {
        criteria.SetQuery(nameof(SysFlowLog.BizId), QueryType.Equal, BizId);
        return Service.QueryFlowLogsAsync(criteria);
    }
}