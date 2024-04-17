using Known.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.WorkFlows;

public class FlowLogGrid : BaseComponent
{
    private TableModel<SysFlowLog> model;

    [Parameter] public string BizId { get; set; }
    [Parameter] public List<SysFlowLog> Logs { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        model = new TableModel<SysFlowLog>(Context) { OnQuery = OnQueryLogs };
        model.AddColumn(c => c.StepName).Width(100).Template(BuildStepName);
        model.AddColumn(c => c.ExecuteBy).Width(100);
        model.AddColumn(c => c.ExecuteTime).Width(180);
        model.AddColumn(c => c.Result).Width(100).Template(BuildResult);
        model.AddColumn(c => c.Note);
    }

    protected override void BuildRender(RenderTreeBuilder builder) => UI.BuildTable(builder, model);

    private void BuildStepName(RenderTreeBuilder builder, SysFlowLog row) => UI.BuildTag(builder, row.StepName);
    private void BuildResult(RenderTreeBuilder builder, SysFlowLog row) => UI.BuildTag(builder, row.Result);

    private async Task<PagingResult<SysFlowLog>> OnQueryLogs(PagingCriteria criteria)
    {
        Logs ??= await Platform.Flow.GetFlowLogsAsync(BizId);
        return new PagingResult<SysFlowLog>(Logs);
    }
}