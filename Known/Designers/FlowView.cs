﻿using Known.Blazor;
using Known.Extensions;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class FlowView : BaseView<FlowInfo>
{
    private TableModel<FlowStepInfo> table;

    internal override async void SetModel(FlowInfo model)
    {
        base.SetModel(model);
        await table.RefreshAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Tab.Items.Add(new ItemModel("Designer.FlowStep") { Content = BuildView });

        table = new(Context);
        table.FixedHeight = "380px";
        table.OnQuery = c =>
        {
            var result = new PagingResult<FlowStepInfo>(Model?.Steps);
            return Task.FromResult(result);
        };
        table.Column(c => c.Pass).Template((b, r) => BuildStatus(b, r.Pass));
        table.Column(c => c.Fail).Template((b, r) => BuildStatus(b, r.Fail));
    }

    private void BuildView(RenderTreeBuilder builder)
    {
        builder.Div("bold", $"{Model?.Name}（{Model?.Id}）");
        UI.BuildTable(builder, table);
    }

    private void BuildStatus(RenderTreeBuilder builder, string value) => UI.BuildTag(builder, value);
}