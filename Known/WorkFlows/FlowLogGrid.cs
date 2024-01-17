﻿using Known.Blazor;
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
        model = new TableModel<SysFlowLog>(Context) { OnQuery = OnQueryLogs };
        model.AddColumn(c => c.StepName);
        model.AddColumn(c => c.ExecuteBy);
        model.AddColumn(c => c.ExecuteTime);
        model.AddColumn(c => c.Result).Template(BuildResult);
        model.AddColumn(c => c.Note);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder) => UI.BuildTable(builder, model);

    private void BuildResult(RenderTreeBuilder builder, SysFlowLog row) => UI.BuildTag(builder, row.Result);

    private async Task<PagingResult<SysFlowLog>> OnQueryLogs(PagingCriteria criteria)
    {
        Logs ??= await Platform.Flow.GetFlowLogsAsync(BizId);
        return new PagingResult<SysFlowLog>(Logs);
    }
}