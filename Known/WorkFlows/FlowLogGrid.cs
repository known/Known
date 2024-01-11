using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.WorkFlows;

public class FlowLogGrid : BaseComponent
{
    private TableModel<SysFlowLog> model;

    [Parameter] public string BizId { get; set; }
    [Parameter] public List<SysFlowLog> Logs { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        model = new TableModel<SysFlowLog> { OnQuery = OnQueryLogs };
        model.Column(c => c.CreateBy).Visible(false);
        model.Column(c => c.CreateTime).Visible(false);
        model.Column(c => c.ModifyBy).Visible(false);
        model.Column(c => c.ModifyTime).Visible(false);
        model.Column(c => c.Result).Template(BuildResult);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder) => UI.BuildTable(builder, model);

    private void BuildResult(RenderTreeBuilder builder, SysFlowLog row) => UI.BuildTag(builder, row.Result);

    private async Task<PagingResult<SysFlowLog>> OnQueryLogs(PagingCriteria criteria)
    {
        Logs ??= await Platform.Flow.GetFlowLogsAsync(BizId);
        return new PagingResult<SysFlowLog>(Logs);
    }
}