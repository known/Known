namespace Known.WorkFlows;

/// <summary>
/// 工作流表单基类。
/// </summary>
/// <typeparam name="TItem">表单数据类型。</typeparam>
public class BaseFlowForm<TItem> : BaseTabForm where TItem : FlowEntity, new()
{
    private IFlowService Service;
    private readonly StepModel step = new();

    /// <summary>
    /// 取得或设置表单配置模型对象。
    /// </summary>
    [Parameter] public FormModel<TItem> Model { get; set; }

    /// <summary>
    /// 异步初始化表单。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<IFlowService>();
        Tab.AddTab("FlowLog", b => b.Component<FlowLogGrid>().Set(c => c.BizId, Model?.Data?.Id).Build());
    }

    /// <summary>
    /// 表单呈现后，异步调用后端流程数据。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            step.Items.Clear();
            var flow = await Service.GetFlowAsync(Context.Current.Id, Model?.Data?.Id);
            var steps = flow.GetFlowStepItems();
            if (steps != null && steps.Count > 0)
                step.Items.AddRange(steps);
            step.Current = flow.Current;
        }
    }

    /// <summary>
    /// 构建表单内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.Steps(step);
        base.BuildForm(builder);
    }
}