namespace Known.Blazor;

/// <summary>
/// 表单基类，继承组件基类。
/// </summary>
public class BaseForm : BaseComponent
{
    /// <summary>
    /// 异步初始化组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        await OnInitFormAsync();
    }

    /// <summary>
    /// 异步初始化表单虚方法，子表单应覆写该方法。
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnInitFormAsync() => Task.CompletedTask;

    /// <summary>
    /// 呈现表单组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Component<KAuthPanel>().Set(c => c.ChildContent, BuildForm).Build();
    }

    /// <summary>
    /// 构建表单组件虚方法，子表单应覆写该方法。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected virtual void BuildForm(RenderTreeBuilder builder) { }
}

/// <summary>
/// 泛型表单基类，继承表单基类。
/// </summary>
/// <typeparam name="TItem">表单对象类型。</typeparam>
public class BaseForm<TItem> : BaseForm where TItem : class, new()
{
    /// <summary>
    /// 取得或设置是否显示【确定】和【取消】操作按钮。
    /// </summary>
    [Parameter] public bool ShowAction { get; set; }

    /// <summary>
    /// 取得或设置泛型表单组件模型实例。
    /// </summary>
    [Parameter] public FormModel<TItem> Model { get; set; }

    /// <summary>
    /// 构建表单组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildForm(RenderTreeBuilder builder)
    {
        if (Model == null)
            return;

        UI.BuildForm(builder, Model);
        if (ShowAction && !Model.IsView)
        {
            builder.FormAction(() =>
            {
                builder.Button(Language.OK, this.Callback<MouseEventArgs>(OnSaveAsync));
                builder.Button(Language.Cancel, this.Callback<MouseEventArgs>(OnCloseAsync), "default");
            });
        }
    }

    private async void OnSaveAsync(MouseEventArgs args) => await Model.SaveAsync();
    private async void OnCloseAsync(MouseEventArgs args) => await Model.CloseAsync();
}

/// <summary>
/// 可编辑的泛型表单基类，继承泛型表单基类。
/// </summary>
/// <typeparam name="TItem">表单对象类型。</typeparam>
public class BaseEditForm<TItem> : BaseForm<TItem> where TItem : class, new()
{
    private bool isEdit = false;

    /// <summary>
    /// 构建表单组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildForm(RenderTreeBuilder builder)
    {
        if (Model == null)
            return;

        Model.IsView = !isEdit;
        BuildFormContent(builder);
    }

    /// <summary>
    /// 构建表单组件虚方法。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected virtual void BuildFormContent(RenderTreeBuilder builder)
    {
        builder.FormPage(() =>
        {
            UI.BuildForm(builder, Model);
            builder.FormPageButton(() => BuildAction(builder));
        });
    }

    /// <summary>
    /// 构建表单【编辑】、【保存】和【取消】操作按钮。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected void BuildAction(RenderTreeBuilder builder)
    {
        if (!isEdit)
        {
            builder.Button(Language.Edit, this.Callback<MouseEventArgs>(e => isEdit = true));
        }
        else
        {
            builder.Button(Language.Save, this.Callback<MouseEventArgs>(OnSaveAsync));
            builder.Button(Language.Cancel, this.Callback<MouseEventArgs>(e => isEdit = false), "default");
        }
    }

    /// <summary>
    /// 异步保存表单数据虚方法。
    /// </summary>
    /// <param name="model">表单数据对象。</param>
    /// <returns>保存结果。</returns>
    protected virtual Task<Result> OnSaveAsync(TItem model) => Result.SuccessAsync("");

    /// <summary>
    /// 保存成功回调虚方法。
    /// </summary>
    /// <param name="result">保存返回结果实例。</param>
    protected virtual void OnSuccess(Result result) { }

    private async void OnSaveAsync(MouseEventArgs arg)
    {
        if (!Model.Validate())
            return;

        var result = await OnSaveAsync(Model.Data);
        UI.Result(result, () =>
        {
            OnSuccess(result);
            isEdit = false;
            return StateChangedAsync();
        });
    }
}

/// <summary>
/// 标签页表单基类，继承表单基类。
/// </summary>
public class BaseTabForm : BaseForm
{
    /// <summary>
    /// 取得标签页表单组件模型实例。
    /// </summary>
    protected TabModel Tab { get; } = new();

    /// <summary>
    /// 构建表单组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildForm(RenderTreeBuilder builder) => UI.BuildTabs(builder, Tab);
}

/// <summary>
/// 步骤表单基类，继承表单基类。
/// </summary>
public class BaseStepForm : BaseForm
{
    /// <summary>
    /// 取得步骤表单组件模型实例。
    /// </summary>
    protected StepModel Step { get; } = new();

    /// <summary>
    /// 构建表单组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildForm(RenderTreeBuilder builder) => UI.BuildSteps(builder, Step);
}