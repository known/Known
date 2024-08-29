namespace Known.Blazor;

public class BaseForm : BaseComponent
{
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        await OnInitFormAsync();
    }

    protected virtual Task OnInitFormAsync() => Task.CompletedTask;

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Component<KAuthPanel>().Set(c => c.ChildContent, BuildForm).Build();
    }

    protected virtual void BuildForm(RenderTreeBuilder builder) { }
}

public class BaseForm<TItem> : BaseForm where TItem : class, new()
{
    [Parameter] public bool ShowAction { get; set; }
    [Parameter] public FormModel<TItem> Model { get; set; }

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

public class BaseEditForm<TItem> : BaseForm<TItem> where TItem : class, new()
{
    private bool isEdit = false;

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        if (Model == null)
            return;

        Model.IsView = !isEdit;
        BuildFormContent(builder);
    }

    protected virtual void BuildFormContent(RenderTreeBuilder builder)
    {
        builder.FormPage(() =>
        {
            UI.BuildForm(builder, Model);
            builder.FormPageButton(() => BuildAction(builder));
        });
    }

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

    protected virtual Task<Result> OnSaveAsync(TItem model) => Result.SuccessAsync("");
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

public class BaseTabForm : BaseForm
{
    protected TabModel Tab { get; } = new();

    protected override void BuildForm(RenderTreeBuilder builder) => UI.BuildTabs(builder, Tab);
}

public class BaseStepForm : BaseForm
{
    protected StepModel Step { get; } = new();

    protected override void BuildForm(RenderTreeBuilder builder) => UI.BuildSteps(builder, Step);
}