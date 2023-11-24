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
    private readonly TabModel tab = new();

    protected List<ItemModel> Tabs { get; } = [];

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        tab.Tabs.Clear();
        if (Tabs.Count > 0)
            tab.Tabs.AddRange(Tabs);
        tab.Tabs.Add(new ItemModel
        {
            Title = "流程记录",
            Content = b => b.Component<FlowLogGrid>().Set(c => c.BizId, Model.Data.Id).Build()
        });
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        UI.BuildTab(builder, tab);
    }
}