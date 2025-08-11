namespace Known.WorkFlows;

/// <summary>
/// 工作流表单。
/// </summary>
/// <typeparam name="TItem">表单数据类型。</typeparam>
public class FlowForm<TItem> : BaseComponent where TItem : FlowEntity, new()
{
    private readonly FlowFormInfo info = new();
    private FlowFormModel flow;

    /// <summary>
    /// 取得或设置表单配置模型对象。
    /// </summary>
    [Parameter] public FormModel<TItem> Model { get; set; }

    /// <summary>
    /// 取得或设置构建表单内容委托。
    /// </summary>
    [Parameter] public Action<RenderTreeBuilder> Content { get; set; }

    /// <summary>
    /// 异步初始化表单。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        InitFlowModel();
    }

    /// <summary>
    /// 呈现流程表单内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-form-content", () => Content?.Invoke(builder));

        if (Model == null || Model.IsView && Model.ViewType == FormViewType.View)
            return;

        if (Model.ViewType != FormViewType.View)
            BuildFlowAction(builder);

        builder.FormAction(() =>
        {
            if (Model.ViewType == FormViewType.Verify)
                builder.Button(new ActionInfo("Assign") { Icon = "" }, this.Callback<MouseEventArgs>(OnAssignAsync));

            builder.Button(new ActionInfo("OK") { Icon = "" }, this.Callback<MouseEventArgs>(OnSaveAsync));
            builder.Button(new ActionInfo("Cancel") { Icon = "" }, this.Callback<MouseEventArgs>(OnCloseAsync));
        });
    }

    private void BuildFlowAction(RenderTreeBuilder builder)
    {
        var action = Language[$"Button.{Model.ViewType}"];
        var title = Language["Title.FlowAction"].Replace("{action}", action);
        builder.Div("kui-flow", () => builder.GroupBox(title, () => builder.Form(flow)));
    }

    private Task OnAssignAsync(MouseEventArgs args) => Task.CompletedTask;//Model.Page.AssignFlowAsync(Model.Data);

    private async Task OnSaveAsync(MouseEventArgs args)
    {
        if (flow != null && !flow.Validate())
            return;

        Result result;
        switch (Model.ViewType)
        {
            case FormViewType.Submit:
                result = await Admin.SubmitFlowAsync(info);
                Model.HandleResult(result);
                break;
            case FormViewType.Verify:
                result = await Admin.VerifyFlowAsync(info);
                Model.HandleResult(result);
                break;
            default:
                await Model.SaveAsync();
                break;
        }
    }

    private Task OnCloseAsync(MouseEventArgs args) => Model.CloseAsync();

    private void InitFlowModel()
    {
        if (Model == null || Model.ViewType == FormViewType.View)
            return;

        info.BizId = Model.Data?.Id;
        flow = new FlowFormModel(this) { Data = info };

        switch (Model.ViewType)
        {
            case FormViewType.Submit:
                flow.AddUserColumn("SubmitTo", "User");
                flow.AddNoteColumn();
                break;
            case FormViewType.Verify:
                flow.AddVerifyColumn();
                flow.AddNoteColumn();
                break;
        }
    }
}