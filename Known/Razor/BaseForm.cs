using Known.Extensions;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

public class BaseForm : BaseComponent
{
    protected override async Task OnInitializedAsync()
    {
        await OnInitFormAsync();
        await base.OnInitializedAsync();
    }

    protected virtual Task OnInitFormAsync() => Task.CompletedTask;
}

public class BaseForm<TItem> : BaseForm where TItem : class, new()
{
    [Parameter] public FormModel<TItem> Model { get; set; }
}

public class BaseFlowForm<TItem> : BaseForm<TItem> where TItem : FlowEntity, new()
{
    private readonly StepModel step = new();
    private readonly TabModel tab = new();

    protected List<ItemModel> Steps { get; } = [];
    protected List<ItemModel> Tabs { get; } = [];

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();

        step.Items.Clear();
        if (Steps.Count > 0)
            step.Items.AddRange(Steps);

        var logs = await Platform.Flow.GetFlowLogsAsync(Model.Data.Id);
        if (logs != null && logs.Count > 0)
        {
            var last = logs.OrderByDescending(l => l.CreateTime).FirstOrDefault();
            if (last.StepName == FlowStatus.StepOver)
                step.Current = step.Items.Last();
        }

        tab.Items.Clear();
        if (Tabs.Count > 0)
            tab.Items.AddRange(Tabs);
        tab.Items.Add(new ItemModel("流程记录")
        {
            Content = b => b.Component<FlowLogGrid>().Set(c => c.Logs, logs).Build()
        });
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        UI.BuildSteps(builder, step);
        UI.BuildTabs(builder, tab);
    }
}