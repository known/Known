namespace Known.Blazor;

/// <summary>
/// 抽象表单基类，继承组件基类。
/// </summary>
public abstract class BaseForm : BaseComponent
{
    /// <summary>
    /// 取得或设置是否是表单页面。
    /// </summary>
    protected bool IsPage { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        await OnInitFormAsync();
    }

    /// <summary>
    /// 异步初始化表单。
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnInitFormAsync() => Task.CompletedTask;

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (IsPage)
            builder.Component<KLoading>().Set(c => c.IsPage, true).Set(c => c.ChildContent, BuildForm).Build();
        else
            BuildForm(builder);
    }

    /// <summary>
    /// 构建表单组件。
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
    /// 取得或设置保存是否关闭对话框，默认关闭。
    /// </summary>
    protected bool SaveClose { get; set; } = true;

    /// <summary>
    /// 取得或设置是否显示【确定】和【取消】操作按钮。
    /// </summary>
    [Parameter] public bool ShowAction { get; set; }

    /// <summary>
    /// 取得或设置泛型表单组件模型实例。
    /// </summary>
    [Parameter] public FormModel<TItem> Model { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model?.Initialize();
    }

    /// <inheritdoc />
    protected override void BuildForm(RenderTreeBuilder builder)
    {
        if (Model == null)
            return;

        builder.Form(Model);
        if (ShowAction && !Model.IsView)
        {
            builder.FormAction(() =>
            {
                builder.Button(Language.OK, this.Callback<MouseEventArgs>(OnSaveAsync));
                builder.Button(Language.Cancel, this.Callback<MouseEventArgs>(OnCloseAsync), "default");
            });
        }
    }

    private Task OnSaveAsync(MouseEventArgs args) => Model.SaveAsync(SaveClose);
    private Task OnCloseAsync(MouseEventArgs args) => Model.CloseAsync();
}

/// <summary>
/// 可编辑的泛型表单基类，继承泛型表单基类。
/// </summary>
/// <typeparam name="TItem">表单对象类型。</typeparam>
public class BaseEditForm<TItem> : BaseForm<TItem> where TItem : class, new()
{
    private bool isEdit = false;

    /// <inheritdoc />
    protected override void BuildForm(RenderTreeBuilder builder)
    {
        if (Model == null)
            return;

        Model.IsView = !isEdit;
        BuildFormContent(builder);
    }

    /// <summary>
    /// 构建表单组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected virtual void BuildFormContent(RenderTreeBuilder builder)
    {
        builder.FormPage(() =>
        {
            builder.Form(Model);
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
    /// 异步保存表单数据。
    /// </summary>
    /// <param name="model">表单数据对象。</param>
    /// <returns>保存结果。</returns>
    protected virtual Task<Result> OnSaveAsync(TItem model) => Model.OnSave?.Invoke(model);

    /// <summary>
    /// 保存成功回调。
    /// </summary>
    /// <param name="result">保存返回结果实例。</param>
    protected virtual void OnSuccess(Result result) { }

    private async Task OnSaveAsync(MouseEventArgs arg)
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
/// 表头表体表单基类。
/// </summary>
/// <typeparam name="THead">表头数据类型。</typeparam>
/// <typeparam name="TList">表体数据类型。</typeparam>
public class BaseForm<THead, TList> : BaseForm<THead> 
    where THead : class, new()
    where TList : class, new()
{
    /// <summary>
    /// 取得默认表体数据对象。
    /// </summary>
    protected virtual TList DefaultList { get; }

    /// <summary>
    /// 取得表体数据列表。
    /// </summary>
    protected List<TList> ListItems { get; } = [];

    /// <summary>
    /// 添加表体数据。
    /// </summary>
    protected virtual void OnAdd()
    {
        ListItems.Add(DefaultList ?? new TList());
        OnRowChanged();
    }

    /// <summary>
    /// 删除表体数据。
    /// </summary>
    /// <param name="row">表体数据。</param>
    protected virtual void OnDelete(TList row)
    {
        ListItems.Remove(row);
        OnRowChanged();
    }

    /// <summary>
    /// 上移表体数据。
    /// </summary>
    /// <param name="row">表体数据。</param>
    protected virtual void OnMoveUp(TList row)
    {
        ListItems.MoveRow(row, true);
        OnRowChanged();
    }

    /// <summary>
    /// 下移表体数据。
    /// </summary>
    /// <param name="row">表体数据。</param>
    protected virtual void OnMoveDown(TList row)
    {
        ListItems.MoveRow(row, false);
        OnRowChanged();
    }

    /// <summary>
    /// 数据行新增、删除、移动改变事件。
    /// </summary>
    protected virtual void OnRowChanged() { }
}

/// <summary>
/// 标签页表单基类，继承表单基类。
/// </summary>
public class BaseTabForm : BaseForm
{
    /// <summary>
    /// 构造函数，创建一个标签页表单实例。
    /// </summary>
    public BaseTabForm()
    {
        Tab = new TabModel(this);
    }

    /// <summary>
    /// 取得标签页表单组件模型实例。
    /// </summary>
    protected TabModel Tab { get; }

    /// <inheritdoc />
    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.Div("kui-form-tab", () => builder.Tabs(Tab));
    }
}

/// <summary>
/// 步骤表单基类，继承表单基类。
/// </summary>
public class BaseStepForm : BaseForm
{
    /// <summary>
    /// 构造函数，创建一个步骤表单实例。
    /// </summary>
    public BaseStepForm()
    {
        Step = new StepModel(this);
    }

    /// <summary>
    /// 取得步骤表单组件模型实例。
    /// </summary>
    protected StepModel Step { get; }

    /// <inheritdoc />
    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.Div("kui-form-step", () => builder.Steps(Step));
    }
}