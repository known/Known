namespace Known.Components;

/// <summary>
/// 步骤表单组件类。
/// </summary>
public class StepForm : BaseComponent
{
    /// <summary>
    /// 取得或设置步骤表单配置模型。
    /// </summary>
    [Parameter] public StepModel Model { get; set; }

    /// <summary>
    /// 取得或设置步骤表单是否是查看模式。
    /// </summary>
    [Parameter] public bool IsView { get; set; }

    /// <summary>
    /// 取得或设置步骤表单是否每一步都保存数据到后台。
    /// </summary>
    [Parameter] public bool IsStepSave { get; set; }

    /// <summary>
    /// 取得或设置步骤表单总步长。
    /// </summary>
    [Parameter] public int? StepCount { get; set; }

    /// <summary>
    /// 取得或设置步骤表单保存数据委托。
    /// </summary>
    [Parameter] public Func<bool, Task<bool>> OnSave { get; set; }

    /// <summary>
    /// 异步初始化步骤表单组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        StepCount ??= Model.Items.Count;
    }

    /// <summary>
    /// 呈现步骤表单组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div($"kui-steps-form {Model.Direction}", () =>
        {
            UI.BuildSteps(builder, Model);
            builder.Div(() =>
            {
                builder.Div("kui-steps-content", () =>
                {
                    if (Model.Items != null && Model.Items.Count > 0)
                        builder.Fragment(Model.Items[Model.Current].Content);
                });
                builder.FormAction(() =>
                {
                    if (Model.Current > 0)
                        builder.Button(Language["Button.Previous"], this.Callback<MouseEventArgs>(OnPrevClickAsync));
                    if (Model.Current < StepCount - 1)
                        builder.Button(Language["Button.Next"], this.Callback<MouseEventArgs>(OnNextClickAsync));
                    if (Model.Current == StepCount - 1 && !IsView)
                        builder.Button(Language["Button.Finish"], this.Callback<MouseEventArgs>(OnCompleteAsync));
                });
            });
        });
    }

    /// <summary>
    /// 设置步骤表单步长。
    /// </summary>
    /// <param name="count">步长。</param>
    public void SetStepCount(int count)
    {
        StepCount = count;
        StateChanged();
    }

    private async void OnPrevClickAsync(MouseEventArgs arg)
    {
        if (IsStepSave)
        {
            if (!await SaveAsync())
                return;
        }

        Model.Current--;
    }

    private async void OnNextClickAsync(MouseEventArgs arg)
    {
        if (IsStepSave)
        {
            if (!await SaveAsync())
                return;
        }

        Model.Current++;
    }

    private async void OnCompleteAsync(MouseEventArgs arg)
    {
        await SaveAsync(true);
    }

    private async Task<bool> SaveAsync(bool isComplete = false)
    {
        if (IsView)
            return true;

        return await OnSave?.Invoke(isComplete);
    }
}