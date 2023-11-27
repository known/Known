using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.WorkFlows;

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

public class FlowForm<TItem> : BaseComponent where TItem : FlowEntity, new()
{
    [Parameter] public FormModel<TItem> Model { get; set; }
    [Parameter] public Action<RenderTreeBuilder> Content { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("form-content", () => Content?.Invoke(builder));

        if (Model.IsView && Model.FormType == FormType.View)
            return;

        if (Model.FormType != FormType.View)
            BuildFlowAction(builder);

        builder.Div("form-action", () =>
        {
            if (Model.FormType == FormType.Verify)
                UI.BuildButton(builder, new ActionInfo("Assign", "") { OnClick = Callback<MouseEventArgs>(OnAssign) });

            UI.BuildButton(builder, new ActionInfo("OK", "") { OnClick = Callback<MouseEventArgs>(OnSave) });
            UI.BuildButton(builder, new ActionInfo("Cancel", "") { OnClick = Callback<MouseEventArgs>(OnClose) });
        });
    }

    private void BuildFlowAction(RenderTreeBuilder builder)
    {
        builder.Div("form-flow", () =>
        {
            var action = Model.FormType.GetDescription();
            builder.Span("title", $"{action}流程");
            switch (Model.FormType)
            {
                case FormType.Submit:
                    //提交给
                    //备注
                    break;
                case FormType.Verify:
                    //指派给、备注
                    //审核结果：通过、退回
                    //退回原因
                    break;
            }
        });
    }

    private void OnAssign(MouseEventArgs args)
    {

    }

    private async void OnSave(MouseEventArgs args)
    {
        var info = new FlowFormInfo();
        switch (Model.FormType)
        {
            case FormType.Submit:
                await Platform.Flow.SubmitFlowAsync(info);
                break;
            case FormType.Verify:
                await Platform.Flow.VerifyFlowAsync(info);
                break;
            default:
                await Model.SaveAsync();
                break;
        }
    }

    private async void OnClose(MouseEventArgs args) => await Model.CloseAsync();
}