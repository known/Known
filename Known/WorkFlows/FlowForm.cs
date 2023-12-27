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

        var logs = await Platform.Flow.GetFlowLogsAsync(Model.Data.Id);
        step.Items.Clear();
        if (Steps.Count > 0)
            step.Items.AddRange(Steps);
        step.Current = GetCurrentStep(logs);

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

    private int GetCurrentStep(List<SysFlowLog> logs)
    {
        if (logs != null && logs.Count > 0)
        {
            var last = logs.OrderByDescending(l => l.CreateTime).FirstOrDefault();
            if (last.StepName == FlowStatus.StepOver && step.Items.Count > 0)
                return step.Items.Count - 1;
        }

        return 0;
    }
}

public class FlowForm<TItem> : BaseComponent where TItem : FlowEntity, new()
{
    private readonly FlowFormInfo info = new();
    private FormModel<FlowFormInfo> flow;

    [Parameter] public FormModel<TItem> Model { get; set; }
    [Parameter] public Action<RenderTreeBuilder> Content { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        InitFlowModel();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-form-content", () => Content?.Invoke(builder));

        if (Model.IsView && Model.FormType == FormType.View)
            return;

        if (Model.FormType != FormType.View)
            BuildFlowAction(builder);

        builder.FormAction(() =>
        {
            if (Model.FormType == FormType.Verify)
                UI.Button(builder, new ActionInfo("Assign", ""), this.Callback<MouseEventArgs>(OnAssign));

            UI.Button(builder, new ActionInfo("OK", ""), this.Callback<MouseEventArgs>(OnSaveAsync));
            UI.Button(builder, new ActionInfo("Cancel", ""), this.Callback<MouseEventArgs>(OnCloseAsync));
        });
    }

    private void BuildFlowAction(RenderTreeBuilder builder)
    {
        builder.Div("kui-form-flow", () =>
        {
            //var action = Model.FormType.GetDescription();
            //builder.Span("title", $"{action}流程");
            UI.BuildForm(builder, flow);
        });
    }

    private void OnAssign(MouseEventArgs args) => Model.Page.AssignFlow(Model.Data);

    private async void OnSaveAsync(MouseEventArgs args)
    {
        if (!flow.Validate())
            return;

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

    private async void OnCloseAsync(MouseEventArgs args) => await Model.CloseAsync();

    private void InitFlowModel()
    {
        if (Model.FormType == FormType.View)
            return;

        info.BizId = Model.Data?.Id;
        flow = new FormModel<FlowFormInfo>(UI);
        flow.Data = info;

        switch (Model.FormType)
        {
            case FormType.Submit:
                flow.AddRow().AddColumn(c => c.User, c =>
                {
                    c.Name = "提交给";
                    c.Required = true;
                    c.Category = "User";
                    c.Type = FieldType.Select;
                });
                flow.AddRow().AddColumn(c => c.Note, c =>
                {
                    c.Name = "备注";
                    c.Type = FieldType.TextArea;
                });
                break;
            case FormType.Verify:
                //指派给、备注
                //审核结果：通过、退回
                //退回原因
                flow.AddRow().AddColumn(c => c.BizStatus, c =>
                {
                    c.Name = "审核结果";
                    c.Category = "通过,退回";
                });
                flow.AddRow().AddColumn(c => c.Note, c =>
                {
                    c.Name = "备注";
                    c.Type = FieldType.TextArea;
                });
                break;
        }
    }
}