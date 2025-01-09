namespace Known.WorkFlows;

/// <summary>
/// 工作流程记录表格组件类。
/// </summary>
public class FlowLogGrid : BaseTable<SysFlowLog>
{
    /// <summary>
    /// 取得或设置业务数据ID。
    /// </summary>
    [Parameter] public string BizId { get; set; }

    /// <summary>
    /// 异步初始化表格。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Table.ShowPager = true;
        Table.FixedHeight = "450px";
        Table.OnQuery = QueryFlowLogsAsync;
        Table.AddColumn(c => c.StepName).Width(100).Template((b, r) => b.Tag(r.StepName));
        Table.AddColumn(c => c.ExecuteBy).Width(100);
        Table.AddColumn(c => c.ExecuteTime).Width(180).DefaultAscend();
        Table.AddColumn(c => c.Result).Width(100).Template((b, r) => b.Tag(r.Result));
        Table.AddColumn(c => c.Note);
    }

    private Task<PagingResult<SysFlowLog>> QueryFlowLogsAsync(PagingCriteria criteria)
    {
        criteria.SetQuery(nameof(SysFlowLog.BizId), QueryType.Equal, BizId ?? "0");
        return Admin.QueryFlowLogsAsync(criteria);
    }
}